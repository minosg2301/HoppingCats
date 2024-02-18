using Doozy.Engine;
using UnityEngine;
using moonNest;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class BaseNavigationListener : MonoBehaviour
{
    public NavigationPathId navigationPathId;

    public GameEvent GameEvent { get; private set; } = null;

    public virtual string PrefixName => "";

    private RectTransform rectTransform;
    public RectTransform RectTransform { get { if (!rectTransform) rectTransform = GetComponent<RectTransform>(); return rectTransform; } }

    void Awake()
    {
        if (navigationPathId > 0)
            GameEvent = GameDefinitionAsset.Ins.FindGameEvent(navigationPathId);
    }

    void OnEnable() { if (GameEvent) RegisterListener(); }

    void OnDisable() { if (GameEvent) UnregisterListener(); }

    protected virtual void Reset()
    {
        // AutoDetectNavigationPathId
        var gameEvent = new GameEvent(PrefixName + gameObject.name);
        foreach (var ge in GameDefinitionAsset.Ins.events)
        {
            if (ge.name == gameEvent.name)
            {
                navigationPathId = ge.id;
                return;
            }
        }

        GameDefinitionAsset.Ins.events.Add(gameEvent);
        navigationPathId = gameEvent.id;
#if UNITY_EDITOR
        EditorUtility.SetDirty(GameDefinitionAsset.Ins);
#endif
    }

    protected virtual void RegisterListener()
    {
        Message.AddListener<GameEventMessage>(GameEvent.name, OnMessage);
    }

    protected virtual void UnregisterListener()
    {
        Message.RemoveListener<GameEventMessage>(GameEvent.name, OnMessage);
    }

    void OnMessage(GameEventMessage message)
    {
        if (GameEvent.name.Equals(message.EventName))
            InvokeEvent(message);
    }

    protected abstract void InvokeEvent(GameEventMessage message);
}
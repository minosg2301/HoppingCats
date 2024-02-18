using Doozy.Engine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using moonNest;

public class NavigationListener : MonoBehaviour
{

    #region Public Variables

    [NonSerialized] public string GameEvent;

    public NavigationPathId navigationPathId;

    /// <summary> UnityEvent executed when this listener has been triggered </summary>

    public UnityEvent<RectTransform> unityEvent;

    #endregion


    private RectTransform rectTransform;
    public RectTransform RectTransform { get { if (!rectTransform) rectTransform = GetComponent<RectTransform>(); return rectTransform; } }

    #region Unity Methods

    private void Awake()
    {
        if (navigationPathId > 0)
            GameEvent = GameDefinitionAsset.Ins.FindGameEvent(navigationPathId).name;
    }

    private void OnEnable() { RegisterListener(); }

    private void OnDisable() { UnregisterListener(); }

    #endregion

    #region Private Methods

    private void RegisterListener()
    {
        Message.AddListener<GameEventMessage>(OnMessage);
    }

    private void UnregisterListener()
    {
        Message.RemoveListener<GameEventMessage>(OnMessage);
        unityEvent.RemoveAllListeners();
    }

    private void OnMessage(GameEventMessage message)
    {
        if (GameEvent.Equals(message.EventName))
            InvokeEvent(message);
    }

    private void InvokeEvent(GameEventMessage message)
    {
        if (!message.HasGameEvent) return;
        if (unityEvent == null) return;

        TryGetComponent<FixContentSizeFitter>(out var fixContentSizeFitter);
        TryGetComponent<ContentSizeFitter>(out var contentSizeFitter);

        if (fixContentSizeFitter || contentSizeFitter)
        {
            StartCoroutine(WaitContentFitSize());
        }

        else unityEvent.Invoke(RectTransform);

        OnNavigationEvent();
    }

    protected virtual void OnNavigationEvent() { }

    IEnumerator WaitContentFitSize()
    {
        var lastRectPos = new Rect();
        while (lastRectPos != RectTransform.rect)
        {
            lastRectPos = RectTransform.rect;
            yield return 0;
        }
        unityEvent.Invoke(RectTransform);
    }

    #endregion
}

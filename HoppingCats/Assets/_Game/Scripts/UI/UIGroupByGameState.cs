using System.Collections.Generic;
using UnityEngine;

public class UIGroupByGameState : MonoBehaviour
{
    public Transform container;
    public CanvasGroup canvasGroup;
    public List<GameState> availableStates;

    protected virtual void OnEnable()
    {
        GameEventManager.Ins.OnGameStateChanged += Set;
    }

    protected virtual void OnDisable()
    {
        GameEventManager.Ins.OnGameStateChanged -= Set;
    }

    public virtual void Set(GameState state)
    {
        var available = availableStates != null && availableStates.Exists(e => e == state);
        if(container) container.gameObject.SetActive(available);
        if (available)
        {
            OnShow();
        }
    }

    protected virtual void OnShow()
    {

    }
}

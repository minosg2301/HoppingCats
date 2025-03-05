using System.Collections.Generic;
using UnityEngine;

public class UIGroupByGameState : MonoBehaviour
{
    public Transform container;
    public CanvasGroup canvasGroup;
    public List<GameState> availableStates;

    private void OnEnable()
    {
        GameEventManager.Ins.OnGameStateChanged += Set;
    }

    private void OnDisable()
    {
        GameEventManager.Ins.OnGameStateChanged -= Set;
    }

    public void Set(GameState state)
    {
        var available = availableStates != null && availableStates.Exists(e => e == state);
        if(container) container.gameObject.SetActive(available);
    }
}

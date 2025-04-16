using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIGroupByGameState : MonoBehaviour
{
    public Transform container;
    public CanvasGroup canvasGroup;
    public List<GameState> availableStates;

    public float animDuration = .3f;

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
        if (available)
        {
            if (container)
            {
                container.transform.localScale = Vector2.zero;
                container.gameObject.SetActive(true);
                container.transform.DOScale(1, animDuration)
                    .SetEase(Ease.InBounce);
            }
            
            OnShow();
        }
        else
        {
            if (container)
            {
                container.transform.DOScale(0, animDuration)
                .SetEase(Ease.OutBounce)
                .OnComplete(() => {
                    container.gameObject.SetActive(false);
                });
            }
        }
    }

    protected virtual void OnShow()
    {

    }
}

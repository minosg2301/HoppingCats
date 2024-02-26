using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using moonNest;
using System;
using DG.Tweening;

public class SwitchViewController : SingletonMono<SwitchViewController>
{
    public RectTransform container;
    public Animator switchViewAnimator;
    public float showDuration = 1.5f;
    public float hideDuration = 1.1f;

    public const string kShow = "show";
    public const string kHide = "hide";

    public static Action onShowDone;
    public static Action onHideDone;

    protected override void Awake()
    {
        base.Awake();
        container.gameObject.SetActive(false);
    }

    public void Show(Action onDone = null)
    {
        container.gameObject.SetActive(true);
        switchViewAnimator.SetTrigger(kShow);

        DOVirtual.DelayedCall(showDuration, () =>
        {
            onShowDone?.Invoke();
            onDone?.Invoke();
            switchViewAnimator.SetTrigger(kHide);
            DOVirtual.DelayedCall(hideDuration, () =>
            {
                OnHideDone();
            });
        });
    }

    private void OnHideDone()
    {
        container.gameObject.SetActive(false);
        onHideDone?.Invoke();
    }
}

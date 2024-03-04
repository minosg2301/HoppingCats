using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using moonNest;
using System;
using DG.Tweening;
using UnityEngine.UI;
public class SwitchViewController : SingletonMono<SwitchViewController>
{
    public RectTransform container;
    public Animator switchViewAnimator;
    public float showDuration = 1.5f;
    public float hideDuration = 1.1f;
    public Image bg;
    public List<Color32> bgColors = new List<Color32>();

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
        GetRandomBGColor();
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

    public void GetRandomBGColor()
    {
        if (bgColors.Count == 0)
        {
            Debug.LogWarning("Dat - No colors available in the list!");
            return;
        }
        Color32 randomColor = bgColors[UnityEngine.Random.Range(0, bgColors.Count)];
        bg.color = randomColor;
    }

    private void OnHideDone()
    {
        container.gameObject.SetActive(false);
        onHideDone?.Invoke();
    }
}

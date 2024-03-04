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


    public const string kShow = "show";
    public const string kHide = "hide";

    public static Action onShowDone;
    public static Action onHideDone;
    [Header("BackGround")]
    public Image bg;
    public List<Color32> bgColors = new List<Color32>();
    [Header("Pillow")]
    public List<Sprite> pillowsSprite = new List<Sprite>();
    public List<Image> pillows = new List<Image>();
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
        //Dat - May cai sprite pillow ko cung size nen goi nhin bi ky`
        //AssignRandomSpritesToPillows();
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

    public void AssignRandomSpritesToPillows()
    {
        Shuffle(pillowsSprite);
        for (int i = 0; i < pillows.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, pillowsSprite.Count);
            pillows[i].sprite = pillowsSprite[randomIndex];
        }
    }
    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

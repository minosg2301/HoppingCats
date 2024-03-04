using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using moonNest;
using System;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using Random = UnityEngine.Random;

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
    public List<Color32> bgColors = new();
    [Header("Pillow")]
    public List<Sprite> pillowsSprite = new();
    public List<Color32> pillowColors = new();
    public List<float> pillowScales = new();
    public List<Image> pillows = new();

    public UIPillow pillowPrefab;
    public RectTransform pillowRecttContainer;
    public List<RectTransform> pillowRectts;
    private List<UIPillow> pillowPoolOjs = new();
    public RectTransform logoGameRectt;

    public AnimationCurve titleAnimationCurve;

#if UNITY_EDITOR
    [Sirenix.OdinInspector.Button]
    private void GetPos()
    {
        pillowRectts.Clear();
        pillowRectts = pillowRecttContainer.GetComponentsInChildren<RectTransform>().ToList();
    }
#endif

    protected override void Awake()
    {
        base.Awake();
        container.gameObject.SetActive(false);

        for(int i = 0; i < pillowRectts.Count; i ++)
        {
            var newPillow = Instantiate(pillowPrefab, pillowPrefab.rectTransform.parent);
            Debug.Log($"T--- Pillow {i}");
            pillowPoolOjs.Add(newPillow);
        }    
    }

    public void Show(Action onDone = null)
    {
        container.gameObject.SetActive(true);
        //switchViewAnimator.SetTrigger(kShow);
        GetRandomBGColor();
        //Dat - May cai sprite pillow ko cung size nen goi nhin bi ky`
        //AssignRandomSpritesToPillows();
        StartCoroutine(DoMoveIn());

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

    public IEnumerator DoMoveIn()
    {
        logoGameRectt.DOScale(0, 0);
        for (int i = 0; i < pillowPoolOjs.Count; i++)
        {
            var currentIndex = i;
            int spriteIndex = Random.Range(0, pillowsSprite.Count);
            pillowPoolOjs[currentIndex].icon.sprite = pillowsSprite[spriteIndex];

            float scaleValue = Random.Range(.7f, 2f);
            pillowPoolOjs[currentIndex].icon.rectTransform.DOScale(scaleValue, 0);

            float rotationValue = Random.Range(-25, 25f);
            pillowPoolOjs[currentIndex].icon.rectTransform.DORotate(new Vector3(0, 0, rotationValue), 0);

            yield return new WaitForSeconds(.06f);

            pillowPoolOjs[currentIndex].rectTransform.SetAsFirstSibling();
            pillowPoolOjs[currentIndex].gameObject.SetActive(true);
            var targetPos = new Vector2(pillowRectts[currentIndex].anchoredPosition.x, pillowRectts[currentIndex].anchoredPosition.y - 100);
            pillowPoolOjs[currentIndex].rectTransform.DOAnchorPos(targetPos, .4f)
                .OnComplete(()=>
                {
                    pillowPoolOjs[currentIndex].rectTransform.DOAnchorPos(pillowRectts[currentIndex].anchoredPosition, .7f);
                    pillowPoolOjs[currentIndex].rectTransform.DOScale(new Vector2(.9f, 1.2f), .3f)
                        .OnComplete(() => 
                        {
                            pillowPoolOjs[currentIndex].rectTransform.DOScale(new Vector2(1f, 1f), .2f);
                        });

                    
                });

            DOVirtual.DelayedCall(1f, () =>
            {
                logoGameRectt.DOScale(1, .4f)
                        .SetEase(titleAnimationCurve);
            });
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

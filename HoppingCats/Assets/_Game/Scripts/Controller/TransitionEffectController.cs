using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using moonNest;
using System;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using Random = UnityEngine.Random;
using Doozy.Engine.UI;

public class TransitionEffectController : SingletonMono<TransitionEffectController>
{
    public RectTransform canvasRectt;
    public UICanvas uiCanvas;
    public RectTransform container;

    public static Action onShowDone;
    public static Action onHideDone;
    [Header("BackGround")]
    public Image bg;
    public List<Color32> bgColors = new();

    [Header("Pillow Properties")]
    public List<Sprite> pillowsSprite = new();
    public List<Color32> pillowColors = new();
    public float pillowScaleMin = 1.4f;
    public float pillowScaleMax = 2;
    public float pillowRotateMin = -20f;
    public float pillowRotateMax = 20f;

    [Header("Animatiom Properties")]
    public float pillowInterval = .04f;
    public float pillowMoveInDuration = 1.1f;
    public float pillowMoveOutDuration = 1.1f;


    [Header("Pillow Gen")]
    public UIPillow pillowPrefab;
    public RectTransform pillowsContainer;
    public RectTransform pillowRecttContainer;
    public List<RectTransform> pillowRectts;
    public List<UIPillow> pillowPoolOjs = new();
    public RectTransform logoGameRectt;

    private List<Vector2> topPosLs = new();
    private List<Vector2> bottomPosLs = new();

    [Header("Anim")]
    public AnimationCurve showAnimCurve;
    public AnimationCurve hideAnimCurve;

#if UNITY_EDITOR
    [Sirenix.OdinInspector.Button]
    private void GetPos()
    {
        pillowRectts.Clear();
        pillowRectts = pillowRecttContainer.GetComponentsInChildren<RectTransform>().ToList();
        pillowRectts.RemoveAt(0);
        pillowRectts.ForEach(e => e.gameObject.name = $"Pillow - { pillowRectts.IndexOf(e)}");
    }

    [Sirenix.OdinInspector.Button]
    private void GetPillowOb()
    {
        pillowPoolOjs.Clear();
        pillowPoolOjs = pillowsContainer.GetComponentsInChildren<UIPillow>(true).ToList();
        pillowPoolOjs.ForEach(e => e.gameObject.name = $"UIPillow - { pillowPoolOjs.IndexOf(e)}");
    }
#endif

    protected override void Awake()
    {
        base.Awake();
        container.gameObject.SetActive(false);

        var hideValue = Screen.height * 3;
        for(int i = 0; i < pillowRectts.Count; i ++)
        {
            //Init Top Pos-----
            topPosLs.Add(new Vector3(pillowRectts[i].anchoredPosition.x, pillowRectts[i].anchoredPosition.y + hideValue));
            //Init Bottom Pos-----
            bottomPosLs.Add(new Vector3(pillowRectts[i].anchoredPosition.x, pillowRectts[i].anchoredPosition.y - hideValue));

            if(i > pillowPoolOjs.Count -1)
            {
                var newPillow = Instantiate(pillowPrefab, pillowPrefab.rectTransform.parent);
                pillowPoolOjs.Add(newPillow);
            }

            pillowPoolOjs[i].rectTransform.DOAnchorPos(topPosLs[i], 0);
        }
    }

    public void Show(Action onDone = null)
    {
        container.gameObject.SetActive(true);
        GetRandomBGColor();
        StartCoroutine(DoMoveIn(()=> 
        {
            onDone?.Invoke();
            StartCoroutine(DoMoveOut(()=>
            {
                container.gameObject.SetActive(false);
            }));
        }));
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
        bg.DOFade(0, 0);
    }

    public IEnumerator DoMoveIn(Action onDone = null)
    {
        logoGameRectt.DOScale(0, 0);
        bg.DOFade(1, .5f);
        yield return new WaitForSeconds(pillowInterval);

        DOVirtual.DelayedCall(pillowMoveInDuration * .8f, () =>
        {
            logoGameRectt.DOScale(1, pillowMoveInDuration * .6f)
                    .SetEase(showAnimCurve);
        });

        for (int i = 0; i < pillowPoolOjs.Count; i++)
        {
            var currentIndex = i;

            int spriteIndex = Random.Range(0, pillowsSprite.Count);
            pillowPoolOjs[currentIndex].icon.sprite = pillowsSprite[spriteIndex];

            float scaleValue = Random.Range(pillowScaleMin, pillowScaleMax);
            pillowPoolOjs[currentIndex].icon.rectTransform.DOScale(scaleValue, 0);

            float rotationValue = Random.Range(pillowRotateMin, pillowRotateMax);
            pillowPoolOjs[currentIndex].icon.rectTransform.DORotate(new Vector3(0, 0, rotationValue), 0);


            pillowPoolOjs[currentIndex].rectTransform.SetAsFirstSibling();
            pillowPoolOjs[currentIndex].gameObject.SetActive(true);
            var targetPos = new Vector2(pillowRectts[currentIndex].anchoredPosition.x, pillowRectts[currentIndex].anchoredPosition.y - 100);
            pillowPoolOjs[currentIndex].rectTransform.DOAnchorPos(targetPos, pillowMoveInDuration * .35f)
                .OnComplete(()=>
                {
                    pillowPoolOjs[currentIndex].rectTransform.DOAnchorPos(pillowRectts[currentIndex].anchoredPosition, pillowMoveInDuration * .65f);
                    pillowPoolOjs[currentIndex].rectTransform.DOScale(new Vector2(.9f, 1.2f), .3f)
                        .OnComplete(() => 
                        {
                            pillowPoolOjs[currentIndex].rectTransform.DOScale(new Vector2(1f, 1f), .2f);
                        });
                });

            yield return new WaitForSeconds(pillowInterval);
        }

        yield return new WaitForSeconds(pillowMoveInDuration + pillowInterval);
        onDone?.Invoke();
        onShowDone?.Invoke();
    }

    public IEnumerator DoMoveOut(Action onDone = null)
    {
        DOVirtual.DelayedCall(pillowMoveOutDuration * .8f, () => 
        {
            logoGameRectt.DOScale(0, pillowMoveOutDuration * .5f)
                .SetEase(hideAnimCurve);
            bg.DOFade(0, 1f);
        });

        for (int i = pillowPoolOjs.Count - 1; i >= 0 ; i--)
        {
            var currentIndex = i;
            var startPos = new Vector2(pillowRectts[currentIndex].anchoredPosition.x, pillowRectts[currentIndex].anchoredPosition.y + 200);
            pillowPoolOjs[currentIndex].rectTransform.DOScale(new Vector2(.9f, 1.2f), .3f)
                        .OnComplete(() =>
                        {
                            pillowPoolOjs[currentIndex].rectTransform.DOScale(new Vector2(1f, 1f), .2f);
                        });

            pillowPoolOjs[currentIndex].rectTransform.DOAnchorPos(startPos, pillowMoveOutDuration * .2f)
                .OnComplete(() =>
                {
                    pillowPoolOjs[currentIndex].rectTransform.DOAnchorPos(bottomPosLs[currentIndex], pillowMoveOutDuration * .8f)
                        .OnComplete(()=> 
                        {
                            pillowPoolOjs[currentIndex].gameObject.SetActive(false);
                            pillowPoolOjs[currentIndex].rectTransform.DOAnchorPos(topPosLs[currentIndex], 0f);
                        });
                });

            yield return new WaitForSeconds(pillowInterval);
        }

        yield return new WaitForSeconds(pillowMoveOutDuration + pillowInterval);
        onDone?.Invoke();
        onHideDone?.Invoke();
    }
}

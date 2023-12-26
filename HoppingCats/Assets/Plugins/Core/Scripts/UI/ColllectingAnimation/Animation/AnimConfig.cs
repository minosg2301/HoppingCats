using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    public abstract class AnimConfig : MonoBehaviour
    {
        public int maxUnit = 10;
        public bool cachePoint = false;

        [Header("Target")]
        public Transform target;
        public bool bumpTarget;
        public float bumpDuration = 0.2f;
        public Vector3 bumpValue = Vector3.one * 0.2f;
        public Ease bumpEase = Ease.OutBounce;

        protected int count = 0;
        protected List<Vector3> Points { get; set; } = new List<Vector3>();

        public int MaxUnit { get; private set; }
        public Sprite Icon { get; internal set; }
        public GameObject Prefab { get; set; }


        private Vector3 targetScale;
        private Image imagePrefab;

        public Action<int, float> onAnimDone;

        protected virtual void Start()
        {
            if(cachePoint) GeneratePoints();
            targetScale = target.localScale;
        }

        protected abstract void GeneratePoints();

        public virtual void Collect(int amount)
        {
            if(!cachePoint || Points.Count == 0)
                GeneratePoints();

            count = 0;
            MaxUnit = Math.Min(amount, maxUnit);
        }

        protected void FlyToTarget(GameObject unit, float duration)
        {
            unit.transform.DOMove(target.position, duration).SetEase(Ease.InSine)
            .OnComplete(() =>
            {
                count++;
                unit.gameObject.SetActive(false);
                onAnimDone?.Invoke(count, (float)count / MaxUnit);

                if(bumpTarget)
                {
                    DOTween.Kill(target);
                    target.localScale = targetScale;
                    target.DOScale(targetScale + bumpValue, bumpDuration).SetEase(bumpEase)
                        .onComplete = () => target.DOScale(targetScale, bumpDuration).SetEase(bumpEase);
                }
            });
        }

        protected GameObject CreateUnit()
        {
            return Prefab ? PoolSystem.RequireObject(Prefab) : CreateImageUnit();
        }

        private GameObject CreateImageUnit()
        {
            if(!imagePrefab)
            {
                RectTransform rect = new GameObject().AddComponent<RectTransform>();
                imagePrefab = rect.gameObject.AddComponent<Image>();
                imagePrefab.preserveAspect = true;
                imagePrefab.gameObject.SetActive(false);
            }

            var image = PoolSystem.RequireObject(imagePrefab);
            image.sprite = Icon;
            image.SetNativeSize();
            return image.gameObject;
        }
    }
}
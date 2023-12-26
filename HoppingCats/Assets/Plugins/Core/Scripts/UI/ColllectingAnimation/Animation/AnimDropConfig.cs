using DG.Tweening;
using System;
using UnityEngine;

namespace moonNest
{
    public class AnimDropConfig : AnimConfig
    {
        [Header("Drop Anim")]
        public float overshoot = 200;
        public float dropY = 100;
        public float paddingX = 50;
        public float expandX = 200;
        public Range scale = new Range(0.7f, 1f);
        public Range dropDuration = new Range(0.5f, 1f);
        public Range flyDuration = new Range(0.5f, 1f);

        private Range expand;

        protected override void Start()
        {
            expand = new Range(-expandX, expandX);
        }

        protected override void GeneratePoints() { }

        public override void Collect(int amount)
        {
            base.Collect(amount);

            for(int i = 0; i < MaxUnit; i++)
            {
                float duration = dropDuration.Random();
                float offsetX = expand.Random();
                offsetX += Math.Sign(offsetX) * paddingX;
                GameObject unit = CreateUnit();
                unit.transform.SetParent(transform, false);
                unit.transform.localPosition = Vector3.zero;
                unit.transform.localScale = Vector3.one * scale.Random();
                unit.transform.DOMoveX(transform.position.x + offsetX, duration).SetEase(Ease.Linear);
                unit.transform.DOMoveY(transform.position.y - dropY, duration).SetEase(Ease.OutBounce, overshoot)
                .OnComplete(() => FlyToTarget(unit, flyDuration.Random()));
            }
        }
    }
}
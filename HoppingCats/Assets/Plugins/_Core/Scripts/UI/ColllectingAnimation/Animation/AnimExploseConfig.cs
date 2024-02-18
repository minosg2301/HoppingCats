using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace moonNest
{
    public class AnimExploseConfig : AnimConfig
    {
        [Header("Explose Anim")]
        public float radius = 100;
        public Range scale = new Range(0.5f, 0.7f);
        public Range exploseDuration = new Range(0.5f, 1f);
        public Range flyDuration = new Range(0.3f, 0.6f);
        public Ease exploseEase = Ease.OutCubic;
        public Ease flyEase = Ease.InSine;

        protected override void GeneratePoints()
        {
            Points.Clear();
            for(int i = 0; i < maxUnit; i++)
            {
                float a = Random.Range(0f, 1f) * 2 * Mathf.PI;
                float r = radius * Mathf.Sqrt(Random.Range(0.5f, 1f));
                Points.Add(new Vector3(r * Mathf.Cos(a), r * Mathf.Sin(a), 0));
            }
        }

        public override void Collect(long amount)
        {
            base.Collect(amount);

            for(int i = 0; i < MaxUnit; i++)
            {
                float duration = exploseDuration.Random();
                Vector3 point = Points[i];
                GameObject unit = CreateUnit();
                unit.transform.SetParent(transform, false);
                unit.transform.localPosition = Vector3.zero;
                unit.transform.localScale = Vector3.one * scale.Random();
                unit.transform.DOMove(transform.position + point, duration).SetEase(exploseEase)
                .OnComplete(() => FlyToTarget(unit, flyDuration.Random()));
            }
        }
    }
}
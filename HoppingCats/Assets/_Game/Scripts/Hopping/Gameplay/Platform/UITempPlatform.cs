using DG.Tweening;
using UnityEngine;

public class UITempPlatform : UIPlatform
{
    [Header("Temp Properties")]
    public float delayDuration = .7f;
    public float hideDuration = .3f;

    public Transform iconTransform;
    public DOTweenAnimation shakeAnimation;

    public ParticleSystem particle;
    public AnimationCurve breakCurve;

    private Tween breakTween;
    private Tween vfxTween;

    public override void Trigger()
    {
        base.Trigger();
        DoBreak();
    }

    public override void Deactive()
    {
        base.Deactive();
        if (breakTween != null) breakTween.Kill();
        if (vfxTween != null) vfxTween.Kill();
    }

    public void DoBreak()
    {
        shakeAnimation.duration = delayDuration + hideDuration;
        shakeAnimation.DOPlay();
        breakTween = DOVirtual.DelayedCall(delayDuration, () => {
            vfxTween = DOVirtual.DelayedCall(delayDuration, ()=> {
                particle.Play();
                isSafe = false;
                onUpdateStatus(this);
            });

            breakTween = iconTransform.transform.DOScale(0f, hideDuration)
                .SetEase(breakCurve)
                .OnComplete(() =>
                {
                    
                });
        });
    }
}
    

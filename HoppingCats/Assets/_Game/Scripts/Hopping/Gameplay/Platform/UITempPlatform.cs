using DG.Tweening;
using UnityEngine;

public class UITempPlatform : UIPlatform
{
    [Header("Temp Properties")]
    public float delayDuration = .7f;
    public float hideDuration = .3f;

    private Tween breakTween;

    public override void Trigger()
    {
        base.Trigger();
        DoBreak();
    }

    public void DoBreak()
    {
        breakTween = transform.DOScale(0, hideDuration)
            .OnStart(()=> {
                isSafe = false;
                onUpdateStatus(this);
            })
            .SetDelay(delayDuration);
    }
}
    

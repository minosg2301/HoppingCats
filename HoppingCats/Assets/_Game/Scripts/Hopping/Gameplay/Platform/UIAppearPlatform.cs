using DG.Tweening;
using UnityEngine;

public class UIAppearPlatform : UIPlatform
{
    [Header("Appear Properties")]
    public float interval = 1f;
    public float appearDuration = .25f;
    public float hideDuration = .4f;

    private bool isAppear = false;
    private bool isActive = true;

    private Tween delayTween;
    private Tween fadeTween;

    public override void SetData(Platform data, int rowIndex)
    {
        base.SetData(data, rowIndex);
        isActive = true;
        isAppear = isSafe;
        platformSprite.DOFade(0, 0);
        DoAppearAnimation();
    }

    private void OnDestroy()
    {
        Deactive();
    }

    public override void Deactive()
    {
        base.Deactive();
        isActive = false;
        if (fadeTween != null) fadeTween.Kill();
        if (delayTween != null) fadeTween.Kill();
    }

    private void DoAppearAnimation()
    {
        if (!isActive) return;
        isAppear = !isAppear;
        var duration = isAppear ? appearDuration : hideDuration;

        if (isAppear)
        {
            fadeTween = platformSprite.DOFade(1, duration)
                .OnComplete(() => {
                    isSafe = isAppear;
                    onUpdateStatus(this);
                });
        }
        else
        {
            fadeTween = platformSprite.DOFade(0, duration)
                .OnStart(() => {
                    isSafe = isAppear;
                    onUpdateStatus(this);
                });
        }
        

        delayTween = DOVirtual.DelayedCall(interval + duration, () => 
        {
            DoAppearAnimation();
        });
    }
}

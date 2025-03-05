using DG.Tweening;
using System;
using UnityEngine;

public class CatAnimationController : MonoBehaviour
{
    [Header("Components")]
    public Transform catUITransform;

    [Header("Animator")]
    public Animator catAnimator;
    private string idleTriggerStr = "idle";
    private string dieTriggerStr = "die";

    [Header("Animation Curvies")]
    public AnimationCurve startJumpCurve;

    public void DoIdle()
    {
        catAnimator.SetTrigger(idleTriggerStr);
    }

    public void DoDie()
    {
        catAnimator.SetTrigger(dieTriggerStr);
    }

    public void DoJumpAnim(Vector3 targetPos, float jumpForce, float jumpDuration, Action onDone = null)
    {
        transform.DOJump(targetPos, jumpForce, 1, jumpDuration)
            .SetEase(startJumpCurve)
            .OnComplete(() => {

                DoAfterFallAnim(jumpDuration * .25f, 0, () => {
                    onDone?.Invoke();
                });
            });
    }

    public void DoAfterFallAnim(float duration, float delay = 0, Action onComplete = null)
    {
        var startAnimDur = duration * .4f;
        var endAnimDur = duration * .6f;

        var defaultPosY = catUITransform.localPosition.y;
        var defaultScaleY = catUITransform.localScale.y;

        var targetScaleY = catUITransform.localScale.y * .7f;
        var targetPosY = catUITransform.localPosition.y - (defaultScaleY - targetScaleY);

        catUITransform.DOLocalMoveY(targetPosY, startAnimDur)
            .SetDelay(delay)
            .OnComplete(() =>
            {
                catUITransform.DOLocalMoveY(defaultPosY, endAnimDur);
            });

        catUITransform.DOScaleY(targetScaleY, startAnimDur)
            .SetDelay(delay)
            .OnComplete(() =>
            {
                catUITransform.DOScaleY(defaultScaleY, endAnimDur);
                onComplete?.Invoke();
            });
    }

    public void DoFlip(bool isRight)
    {
        Vector3 scale = catUITransform.localScale;
        if (isRight) scale.x = -Mathf.Abs(scale.x);
        else scale.x = Mathf.Abs(scale.x);
        catUITransform.localScale = scale;
    }
}

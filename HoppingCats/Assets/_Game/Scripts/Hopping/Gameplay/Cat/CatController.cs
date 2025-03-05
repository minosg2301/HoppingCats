using DG.Tweening;
using System;
using UnityEngine;

public enum JumpType
{
    Left,
    Right
}

public class CatController : MonoBehaviour
{
    [Header("Components")]
    public Transform catUITransform;
    public GameController gameController;

    [Header("Properties")]
    public float jumpForce = 1f;
    public float jumpDuration = 0.2f;

    [Header("Animation Curvies")]
    public AnimationCurve startJumpCurve;

    private bool isJumping = false;

    private UIPlatform platFormGrounding;

    public bool IsJumping => isJumping;

    public void DoJump(JumpType type, Action onStartJump = null, Action onEndJump = null)
    {
        isJumping = true;
        ClearPlatform();
        onStartJump?.Invoke();

        DoFlip(type == JumpType.Right);

        Vector3 targetPos = transform.position;
        targetPos.x += type == JumpType.Right ? 1 : -1;
        targetPos.y += 2;

        
        transform.DOJump(targetPos, jumpForce, 1, jumpDuration)
            .SetEase(startJumpCurve)
            .OnComplete(() => {

                DoAfterFallAnim(jumpDuration * .25f,  0, ()=> {
                    isJumping = false;
                    onEndJump?.Invoke();
                });
            });

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!platFormGrounding)
        {
            var nextPlatform = other.gameObject.GetComponent<UIPlatform>();
            if (nextPlatform)
            {
                platFormGrounding = nextPlatform;
                if (!platFormGrounding.Status.isSafe)
                {
                    gameController.LoseHandle();
                    return;
                }
                platFormGrounding.onUpdateStatus = OnPlatFormUpdate;
            }
        }
    }

    private void OnPlatFormUpdate(UIPlatformStatus uiPlatformStatus)
    {
        if (!uiPlatformStatus.isSafe)
        {
            gameController.LoseHandle();
        }
    }

    private void ClearPlatform()
    {
        if (platFormGrounding)
        {
            platFormGrounding.onUpdateStatus = null;
        }
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



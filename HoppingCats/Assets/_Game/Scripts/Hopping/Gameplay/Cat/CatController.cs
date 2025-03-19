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
    public CatAnimationController animationController;

    [Header("Properties")]
    public float jumpForce = 1f;
    public float jumpDuration = 0.2f;

    private bool isJumping = false;
    private UIPlatform platFormGrounding;

    public bool IsJumping => isJumping;

    public void DoJump(JumpType type, Action onStartJump = null, Action onEndJump = null)
    {
        isJumping = true;
        ClearPlatform();
        onStartJump?.Invoke();

        animationController.DoFlip(type == JumpType.Right);

        Vector3 targetPos = transform.position;
        targetPos.x += type == JumpType.Right ? 1 : -1;
        targetPos.y += 2;

        animationController.DoJumpAnim(targetPos, jumpForce, jumpDuration, 
            () => {
                //jump done
                isJumping = false;
                onEndJump?.Invoke();
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
                if (!platFormGrounding.IsSafe)
                {
                    DoLose();
                    return;
                }
                else
                {
                    GameEventManager.Ins.OnAddScore();
                }
                platFormGrounding.onUpdateStatus = OnPlatFormUpdate;
            }
        }
    }

    private void OnPlatFormUpdate(UIPlatform uiPlatform)
    {
        if (!uiPlatform.IsSafe)
        {
            DoLose();
        }
    }

    private void DoLose()
    {
        platFormGrounding.onUpdateStatus = null;
        platFormGrounding = null;
        GameController.Ins.LoseHandle();
    }

    private void ClearPlatform()
    {
        if (platFormGrounding)
        {
            platFormGrounding.onUpdateStatus = null;
            platFormGrounding = null;
        }
    }
}



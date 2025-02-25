using DG.Tweening;
using UnityEngine;

public enum MoveType
{
    Left,
    Right
}

public class CatController : MonoBehaviour
{
    [Header("Components")]
    public Collider2D catCollider;
    public Transform catUITransform;

    [Header("Properties")]
    public float jumpPower = 1f;
    public float jumpDuration = 0.2f;

    private bool isMoving = false;

    public bool IsMoving => isMoving;

    public void MoveRight()
    {
        isMoving = true;
        Turn(true);
        Vector3 targetPos = transform.position;
        targetPos.x += 1;
        targetPos.y += 2;
        transform.DOJump3D(targetPos, -jumpPower, jumpDuration).OnComplete(() => isMoving = false);
    }

    public void MoveLeft()
    {
        isMoving = true;
        Turn(false);
        Vector3 targetPos = transform.position;
        targetPos.x -= 1;
        targetPos.y += 2;
        transform.DOJump3D(targetPos, -jumpPower, jumpDuration).OnComplete(() => isMoving = false);
    }
    private void Turn(bool faceRight)
    {
        Vector3 scale = catUITransform.localScale;
        if (faceRight) scale.x = -Mathf.Abs(scale.x);
        else scale.x = Mathf.Abs(scale.x);
        catUITransform.localScale = scale;
    }
}



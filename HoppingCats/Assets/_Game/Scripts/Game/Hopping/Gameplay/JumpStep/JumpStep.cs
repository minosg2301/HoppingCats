using UnityEngine;

public class JumpStep : MonoBehaviour
{
    public JumpConfig config;
    public int index;
    public Vector2 jumpPosition;


    private bool deadJump;
    public bool DeadJump => deadJump;

    protected virtual void Active()
    {

    }

    protected virtual void Deactive()
    {

    }

    protected virtual void ApplyJumpStep()
    {

    }
}

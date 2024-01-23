using UnityEngine;

public class JumpStep : MonoBehaviour
{
    public JumpStepData data;

    private bool deadJump;
    public bool DeadJump => deadJump;

    public JumpStep(JumpStepData data)
    {
        this.data = data;
    }

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

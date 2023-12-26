using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpStep : MonoBehaviour
{
    public JumpConfig config;
    public Vector2 jumpPosition;
    public bool deadJump;

    protected virtual void Active()
    {

    }

    protected virtual void Deactive()
    {

    }

    protected virtual void SetDeadjump(bool value)
    {
        deadJump = value;
    }
}

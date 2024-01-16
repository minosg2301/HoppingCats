using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearJumpStep : JumpStep
{
    public AppearJumpStep(int index, JumpConfig config) : base(index, config)
    {

    }

    protected override void Active()
    {
        base.Active();
    }

    protected override void Deactive()
    {
        base.Deactive();
    }
}

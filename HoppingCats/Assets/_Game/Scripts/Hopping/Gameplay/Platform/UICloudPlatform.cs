using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICloudPlatform : UIPlatform
{
    [Header("Temp Properties")]
    public float hideDuration = .3f;

    public override void Trigger()
    {
        base.Trigger();
        DoBreak();
    }

    public void DoBreak()
    {
        transform.DOScale(0, hideDuration);
    }
}

using moonNest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseViewExt : BaseView
{
    protected override void OnShow()
    {
        base.OnShow();
        View.CanvasGroup.blocksRaycasts = true;
    }

    protected override void OnHide()
    {
        base.OnHide();
    }
}

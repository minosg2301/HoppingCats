using Doozy.Engine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using moonNest;


public class UINavigation : MonoBehaviour
{
    public NavigationId navigationId;
    public UIButton navigationButton;

    private void Awake()
    {
        if (navigationButton) navigationButton.OnClick.OnTrigger.Event.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (navigationId > 0) NavigationHandler.Ins.DoNavigate(navigationId);
    }
}

using Doozy.Engine;
using UnityEngine;
using moonNest;

[RequireComponent(typeof(UITabItem))]
public class NavigationTabItem : BaseNavigationListener
{
    public UITabItem tabItem;

    public override string PrefixName => "GoToTab";

    protected override void Reset()
    {
        base.Reset();

        if (!tabItem)
            tabItem = GetComponent<UITabItem>();
    }

    protected override void InvokeEvent(GameEventMessage message)
    {
        if (tabItem) tabItem.InvokeClick();
    }
}
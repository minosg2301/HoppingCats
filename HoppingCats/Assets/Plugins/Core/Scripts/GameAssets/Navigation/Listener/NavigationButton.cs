using Doozy.Engine;
using Doozy.Engine.UI;
using UnityEngine;

[RequireComponent(typeof(UIButton))]
public class NavigationButton : BaseNavigationListener
{
    public UIButton button;

    protected override void Reset()
    {
        base.Reset();

        if (!button)
            button = GetComponent<UIButton>();
    }

    protected override void InvokeEvent(GameEventMessage message)
    {
        if (button) button.OnClick.OnTrigger.Event.Invoke();
    }
}
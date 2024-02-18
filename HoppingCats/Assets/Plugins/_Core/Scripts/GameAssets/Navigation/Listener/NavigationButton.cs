using Doozy.Engine;
using Doozy.Engine.UI;
using UnityEngine;

[RequireComponent(typeof(UIButton))]
public class NavigationButtonListener : BaseNavigationListener
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
        if (button)
        {
            button.OnClick.OnTrigger.SendGameEvents(button.gameObject);
            button.OnClick.OnTrigger.InvokeAction(button.gameObject);
            button.OnClick.OnTrigger.InvokeUnityEvent();
            button.NotifySystemOfTriggeredBehavior(button.OnClick.BehaviorType);
        }
    }
}
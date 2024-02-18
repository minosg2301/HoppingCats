using Doozy.Engine.UI;
using UnityEngine;
using moonNest;

public class UINavigationTrigger : MonoBehaviour
{
    public NavigationId navigationId;
    public UIButton navigationButton;

    void Awake()
    {
        if (navigationButton) navigationButton.OnClick.OnTrigger.Event.AddListener(OnClick);
    }

    void OnDestroy()
    {
        if (navigationButton) navigationButton.OnClick.OnTrigger.Event.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        if (navigationId > 0) NavigationHandler.Ins.DoNavigate(navigationId);
    }
}

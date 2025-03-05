using Doozy.Engine;
using Doozy.Engine.UI;
using UnityEngine;

public class InGameView : BaseViewExt
{
    public UIButton lobbyButton;
    public UIButton shopButton;

    public RectTransform starGroup;
    public RectTransform eggGroup;
    public RectTransform healthGroup;
    public RectTransform scoreGroup;

    public static InGameView Ins;

    public static void Show()
    {
        GameEventMessage.SendEvent(DoozyEventDefine.kGoToInGame);
    }

    protected override void Awake()
    {
        base.Awake();
        if (!Ins) Ins = this;
        if(lobbyButton) lobbyButton.OnClick.OnTrigger.Event.AddListener(OnLobbyClick);
        ShowReadyUI();
    }

    protected override void OnShow()
    {
        base.OnShow();
        GameEventManager.Ins.OnSetupLevel();
    }

    public void ShowReadyUI()
    {
        healthGroup.gameObject.SetActive(false);
        lobbyButton.gameObject.SetActive(true);
        shopButton.gameObject.SetActive(true);
        eggGroup.gameObject.SetActive(true);
        scoreGroup.gameObject.SetActive(false);
    }

    public void ShowIngameUI()
    {
        scoreGroup.gameObject.SetActive(true);
        healthGroup.gameObject.SetActive(true);
        lobbyButton.gameObject.SetActive(false);
        shopButton.gameObject.SetActive(false);
        eggGroup.gameObject.SetActive(false);
    }
    private void OnLobbyClick()
    {
        TransitionEffectController.Ins.Show(()=>
        {
            MainView.Show();
        });
    }
}

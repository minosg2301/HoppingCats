using Doozy.Engine;
using Doozy.Engine.UI;

public class InGameView : BaseViewExt
{
    public UIButton lobbyButton;
    public UIButton shopButton;

    public static void Show()
    {
        GameEventMessage.SendEvent(DoozyEventDefine.kGoToInGame);
    }

    protected override void Awake()
    {
        base.Awake();
        if(lobbyButton) lobbyButton.OnClick.OnTrigger.Event.AddListener(OnLobbyClick);
    }

    protected override void OnShow()
    {
        base.OnShow();
        GameEventManager.Ins.OnSetupLevel();
    }

    private void OnLobbyClick()
    {
        TransitionEffectController.Ins.Show(()=>
        {
            MainView.Show();
        });
    }
}

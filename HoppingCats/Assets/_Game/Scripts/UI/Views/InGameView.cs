using Doozy.Engine;
using Doozy.Engine.UI;

public class InGameView : BaseViewExt
{
    public UIButton lobbyButton;
    public static void Show()
    {
        GameEventMessage.SendEvent(DoozyEventDefine.kGoToInGame);
    }
    protected override void Awake()
    {
        base.Awake();
        if(lobbyButton) lobbyButton.OnClick.OnTrigger.Event.AddListener(() => MainView.Show());
    }
}

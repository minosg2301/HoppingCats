using Doozy.Engine;
using Doozy.Engine.UI;

public class MainView : BaseViewExt
{
    public UIButton playButton;

    public static void Show()
    {
        GameEventMessage.SendEvent(DoozyEventDefine.kGoToMain);
    }

    protected override void Awake()
    {
        base.Awake();
        if (playButton) playButton.OnClick.OnTrigger.Event.AddListener(() => InGameView.Show());
    }
}

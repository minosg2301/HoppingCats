using Doozy.Engine;

public class MainView : BaseViewExt
{
    public static void Show()
    {
        GameEventMessage.SendEvent(DoozyEventDefine.kGoToMain);
    }    
}

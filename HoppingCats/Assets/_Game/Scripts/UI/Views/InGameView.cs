using Doozy.Engine;

public class InGameView : BaseViewExt
{
    public static void Show()
    {
        GameEventMessage.SendEvent(DoozyEventDefine.kGoToInGame);
    }    
}

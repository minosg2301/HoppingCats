using moonNest;

public class GameBootComplete : BootStep
{
    public override void OnStep()
    {
        GameEventManager.Ins.OnFirstLoadingComplete();
        Complete();
    }
}

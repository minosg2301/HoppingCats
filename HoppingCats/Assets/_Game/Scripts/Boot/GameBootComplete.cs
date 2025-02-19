using moonNest;
using System;

public class GameBootComplete : BootStep
{
    public static Action onComplete = delegate { };

    public override void OnStep()
    {
        onComplete();
        Complete();
    }
}

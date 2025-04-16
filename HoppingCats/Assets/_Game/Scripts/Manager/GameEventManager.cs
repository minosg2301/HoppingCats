using moonNest;
using System;

public class GameEventManager : SingletonMono<GameEventManager>
{
    public Action OnFirstLoadingComplete = delegate { };

    public Action<GameState> OnGameStateChanged = delegate { };

    //game play
    public Action OnSetupLevel = delegate { };
    public Action OnGameLose = delegate { };
    public Action OnStartGame = delegate { };
    public Action OnEndGame = delegate { };
    public Action OnAddScore = delegate { };

    public Action<int> OnAddHealth = delegate { };
    public Action<int> OnAddStar = delegate { };

    public Action<SkinConfig> OnSkinUpdate = delegate { };

}

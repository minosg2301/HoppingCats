using moonNest;
using UnityEngine;

public class GameStateManager : SingletonMono<GameStateManager>
{
    private GameState currentGameState;
    public GameState CurrentGameState => currentGameState;

    public void ChangeGameState(GameState gameState)
    {
        currentGameState = gameState;
        GameEventManager.Ins.OnGameStateChanged(gameState);
    }
}

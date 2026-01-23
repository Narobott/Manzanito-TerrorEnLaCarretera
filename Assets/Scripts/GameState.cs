using UnityEngine;
using UnityEngine.Events;

public class GameState : MonoBehaviour
{
    public enum GameStateEnum
    {
        StartScreen,
        Game,
        LoseScreen,
        SettingsScreen
    }

    public GameStateEnum gameState = GameStateEnum.Game;

    public UnityEvent<GameStateEnum> gameStateChanged;

    public void SetGameState(GameStateEnum _gameState)
    {
        gameState = _gameState;
        gameStateChanged.Invoke(gameState);
    }


    private float GameTime = 0;

    public float GetGameTime()
    {
        return GameTime;
    }

    private void FixedUpdate()
    {
        if ( gameState.Equals(GameStateEnum.Game))
        {
            GameTime += Time.fixedDeltaTime;
        }
    }

    private int playerPositionIndex;

    public int getPlayerPositionIndex()
    {
        return playerPositionIndex;
    }

    public void setPlayerPositionIndex(int index)
    {
        playerPositionIndex = index;
    }

}

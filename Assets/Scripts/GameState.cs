using System;
using System.Collections;
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

    public GameStateEnum gameState = GameStateEnum.StartScreen;

    public UnityEvent<GameStateEnum> gameStateChanged;

    public void SetGameState(GameStateEnum _gameState)
    {
        gameState = _gameState;
        gameStateChanged.Invoke(gameState);

        switch (gameState)
        {
            case GameStateEnum.StartScreen:
                break;

            case GameStateEnum.Game:
                gameOverPanel.SetActive(false);
                break;

            case GameStateEnum.LoseScreen:
                bWasFirstSwipeDetected = false;
                openSettingsButton.SetActive(false);
                gameOverPanel.SetActive(true);
                break;

            case GameStateEnum.SettingsScreen:
                settingsScreenPanel.SetActive(true);
                break;
        }

    }



    private void Awake()
    {
        SwipeDetection swipeDetection = GetComponent<SwipeDetection>();
        swipeDetection.swipePerformed.AddListener(FirstSwipeDetected);
    }

    bool bWasFirstSwipeDetected = false;

    private void FirstSwipeDetected(Vector2 direction)
    {
        if (!bWasFirstSwipeDetected && gameState == GameStateEnum.StartScreen)
        {
            SetGameState(GameStateEnum.Game);
            bWasFirstSwipeDetected= true;
        }
    }

    public GameStateEnum prevGameState;
    public void OpenSettings()
    {
        prevGameState = gameState;
        SetGameState(GameStateEnum.SettingsScreen);
        closeSettingsButton.SetActive(true);
        openSettingsButton.SetActive(false);
    }

    public void CloseSettings()
    {
        SetGameState(prevGameState);
        closeSettingsButton.SetActive(false);
        openSettingsButton.SetActive(true);
        settingsScreenPanel.SetActive(false);
    }

    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject settingsScreenPanel;
    [SerializeField] GameObject openSettingsButton;
    [SerializeField] GameObject closeSettingsButton;


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

    private int points;

    public UnityEvent pointsIncreased;
    public void increasePoints(int _points)
    {
        this.points += _points;
        pointsIncreased.Invoke();

        if (PlayerPrefs.HasKey("SavedHighScore"))
        {
            if (points > PlayerPrefs.GetInt("SavedHighScore"))
            {
                PlayerPrefs.SetInt("SavedHighScore", this.points);
            }
        }
        else
        {
            PlayerPrefs.SetInt("SavedHighScore", this.points);
        }
    }
    public int getPoints()
    {
        return points;
    }

}

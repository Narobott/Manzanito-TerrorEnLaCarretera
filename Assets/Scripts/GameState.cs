using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameState : MonoBehaviour
{
    public enum GameStateEnum
    {
        StartScreen,
        Game,
        LoseScreen,
        SettingsScreen,
        ParryTutorial,
        ImpactFrame
    }

    public GameStateEnum gameState = GameStateEnum.StartScreen;

    public UnityEvent<GameStateEnum> gameStateChanged;

    public void SetGameState(GameStateEnum _gameState)
    {
        GameStateEnum prevGameState = gameState;
        gameState = _gameState;
        gameStateChanged.Invoke(gameState);

        switch (gameState)
        {
            case GameStateEnum.StartScreen:
                enemySpawner.ResetDificultyLevel();
                gameOverPanel.SetActive(false);
                vignete.SetActive(true);
                player.ResetCharacterPosition();
                SetPoints(0);
                ResetGameTime();
                player.gameObject.GetComponent<CharacterStats>().SetCharacterHealth(1);
                openSettingsButton.SetActive(true);
                startGamePannel.SetActive(true);
                break;

            case GameStateEnum.Game:
                vignete.SetActive(false);
                startGamePannel.SetActive(false);
                break;

            case GameStateEnum.LoseScreen:
                bWasFirstSwipeDetected = false;
                vignete.SetActive(true);
                openSettingsButton.SetActive(false);
                gameOverPanel.SetActive(true);
                break;

            case GameStateEnum.SettingsScreen:
                vignete.SetActive(true);
                settingsScreenPanel.SetActive(true);
                startGamePannel.SetActive(false);
                break;

            case GameStateEnum.ImpactFrame:
                StartCoroutine(fullscreenEffectsManager.InvokeImpactFrame(prevGameState));
                break;
        }

    }


    private EnemySpawner enemySpawner;
    [SerializeField] private FullscreenEffectsManager fullscreenEffectsManager;
    private void Awake()
    {
        SwipeDetection swipeDetection = GetComponent<SwipeDetection>();
        swipeDetection.swipePerformed.AddListener(FirstSwipeDetected);
        swipeDetection.pressPerformed.AddListener(PressDetected);

        enemySpawner = GetComponent<EnemySpawner>();

        SetGameState(GameStateEnum.StartScreen);
    }

    bool bWasFirstSwipeDetected = false;

    private void FirstSwipeDetected(Vector2 direction)
    {
        if (!bWasFirstSwipeDetected && gameState == GameStateEnum.StartScreen && Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
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

    private void PressDetected()
    {
        if (gameState == GameStateEnum.LoseScreen)
        {
            SetGameState (GameStateEnum.StartScreen);
        }
    }

    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject settingsScreenPanel;
    [SerializeField] GameObject startGamePannel;
    [SerializeField] GameObject openSettingsButton;
    [SerializeField] GameObject closeSettingsButton;
    [SerializeField] GameObject vignete;
    [SerializeField] CharacterMovement player;


    private float GameTime = 0;

    public float GetGameTime()
    {
        return GameTime;
    }

    private void ResetGameTime()
    {
        GameTime = 0;
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
    [SerializeField] AudioSource pointsAudioSource;
    [SerializeField] TextMeshPro pointsText;
    private void SetPoints(int points)
    {
        this.points = points;
        pointsText.text = points.ToString();
    }

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

        pointsAudioSource.Play();
    }
    public int getPoints()
    {
        return points;
    }

}

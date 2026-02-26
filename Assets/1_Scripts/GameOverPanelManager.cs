using GooglePlayGames;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.Impl;

public class GameOverPanelManager : MonoBehaviour
{

    [SerializeField] private TextMeshPro HighScore;
    [SerializeField] private TextMeshPro NewScore;
    [SerializeField] private GameObject Systems;
    [SerializeField] private AudioSource audioSource;
    private GameState gameState;

    private void Awake()
    {
        gameState = Systems.GetComponent<GameState>();
    }


    private void OnEnable()
    {

        int score = PlayerPrefs.GetInt("SavedHighScore");

        HighScore.text = score.ToString();
        PlayGamesPlatform.Instance.ReportScore(
        score,
        "CgkltejH2vMXEAIQAA",
        (bool success) =>
        {
            if (success)
                Debug.Log("New high score published correctly");
            else
                Debug.Log("Failed to publish high score");
        });
        StartCoroutine(CountPointsAnimation());
    }

    private IEnumerator CountPointsAnimation()
    {
        int target = gameState.getPoints();
        float duration = 1.2f;
        float elapsed = 0f;
        int i = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Exponential ease-in
            float eased = Mathf.Pow(t, 3f);

            int value = Mathf.RoundToInt(Mathf.Lerp(0, target, eased));
            NewScore.text = value.ToString();

            if(i % 5 == 0)
            {
                audioSource.Play();
            }
            i++;

            yield return null;
        }

        NewScore.text = target.ToString();
    }

}

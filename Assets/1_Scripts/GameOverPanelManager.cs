using System.Collections;
using TMPro;
using UnityEngine;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using System;

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
        HighScore.text = PlayerPrefs.GetInt("SavedHighScore").ToString();
        PlayGamesPlatform.Instance.ReportScore(PlayerPrefs.GetInt("SavedHighScore"), "high-score", callback);
        StartCoroutine(CountPointsAnimation());
    }

    private void callback(bool obj)
    {
        if (obj)
        {
            Debug.Log("New high score published correctly");
        }
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

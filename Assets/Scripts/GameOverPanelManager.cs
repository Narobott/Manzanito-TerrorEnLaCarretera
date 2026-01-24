using System.Collections;
using TMPro;
using UnityEngine;

public class GameOverPanelManager : MonoBehaviour
{

    [SerializeField] private TextMeshPro HighScore;
    [SerializeField] private TextMeshPro NewScore;
    [SerializeField] private GameObject Systems;
    private int newScoreCount = 0;
    private GameState gameState;

    private void Awake()
    {
        gameState = Systems.GetComponent<GameState>();
    }


    private void OnEnable()
    {
        HighScore.text = PlayerPrefs.GetInt("SavedHighScore").ToString();
        StartCoroutine(CountPointsAnimation());
    }

    private IEnumerator CountPointsAnimation()
    {
        int target = gameState.getPoints();
        float duration = 1.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Exponential ease-in
            float eased = Mathf.Pow(t, 3f);

            int value = Mathf.RoundToInt(Mathf.Lerp(0, target, eased));
            NewScore.text = value.ToString();

            yield return null;
        }

        NewScore.text = target.ToString();
    }

}

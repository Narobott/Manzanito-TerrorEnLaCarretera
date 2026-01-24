using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GameState gameState;

    private void Awake()
    {
        gameState = GetComponent<GameState>();
        gameState.pointsIncreased.AddListener(PlayPointsAnimation);
    }

    [SerializeField]
    private Vector3Int[] horizontalSnapPositions = null;

    public Vector3Int[] GetHorizontalSnapPositions()
    {
        return horizontalSnapPositions;
    }

    [SerializeField]
    public int enemyDodgePoints = 300;

    [SerializeField]
    public TextMeshPro PointsText;

    private void PlayPointsAnimation()
    {
        PointsText.gameObject.GetComponent<Animator>().SetTrigger("PlayIncreasePoints");
    }
}

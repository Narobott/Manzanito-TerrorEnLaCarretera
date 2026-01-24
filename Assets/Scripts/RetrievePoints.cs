using TMPro;
using UnityEngine;

public class RetrievePoints : MonoBehaviour
{

    Vector3 startPosition;
    Vector3 endPosition = new(0f,-9f,0);
    float movementAlpha = 0.0f;
    float speedMultiplier = 3.0f;
    int enemyDodgePoints;
    [SerializeField]Color TextColor;
    TextMeshPro TMP;
    GameState gameState;
    GameManager gameManager;

    public bool bIsCloseCall;

    void Start()
    {
        gameState = GameObject.Find("Systems").GetComponent<GameState>();
        gameManager = GameObject.Find("Systems").GetComponent<GameManager>();

        enemyDodgePoints = bIsCloseCall ? gameManager.enemyCloseCallDodgePoints : gameManager.enemyDodgePoints;

        TMP = GetComponent<TextMeshPro>();
        TMP.text = enemyDodgePoints.ToString();
        TMP.color = TextColor;

        startPosition = gameObject.transform.position;
    }

    private void Update()
    {
        movementAlpha += Time.deltaTime * speedMultiplier;
        transform.position = Vector3.Lerp(startPosition, endPosition, movementAlpha);
        if (movementAlpha > .99f)
        {
            GameObject.Find("Systems").GetComponent<GameState>().increasePoints(enemyDodgePoints);
            gameManager.PointsText.text = gameState.getPoints().ToString();
            Destroy(gameObject);
        }
    }

}

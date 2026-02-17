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

    private bool bIsCloseCall;
    private bool bIsParry;

    public void SetIsCloseCall(bool isCloseCall)
    {
        this.bIsCloseCall = isCloseCall;
        if (isCloseCall )
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void SetIsParry(bool isParry)
    {
        this.bIsParry = isParry;
    }

    void Start()
    {
        gameState = GameObject.Find("Systems").GetComponent<GameState>();
        gameManager = GameObject.Find("Systems").GetComponent<GameManager>();

        enemyDodgePoints = gameManager.enemyDodgePoints;

        if (bIsCloseCall)
        {
            enemyDodgePoints = gameManager.enemyCloseCallDodgePoints;
        }

        if (bIsParry)
        {
            enemyDodgePoints = gameManager.parryPoints;
        }

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

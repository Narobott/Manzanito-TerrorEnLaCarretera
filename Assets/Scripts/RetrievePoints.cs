using TMPro;
using UnityEngine;

public class RetrievePoints : MonoBehaviour
{

    Vector3 startPosition;
    Vector3 endPosition = new(3.5f,9f,0);
    float movementAlpha = 0.0f;
    float movementSpeed = 0.1f;
    int enemyDodgePoints;

    void Start()
    {
        enemyDodgePoints = GameObject.Find("Systems").GetComponent<GameManager>().enemyDodgePoints;
        GetComponent<TextMeshPro>().text = enemyDodgePoints.ToString();

        startPosition = gameObject.transform.position;
    }

    private void Update()
    {
        movementAlpha += Time.deltaTime;
        transform.position = Vector3.Lerp(startPosition, endPosition, movementAlpha);
        if (endPosition == transform.position)
        {
            GameObject.Find("Systems").GetComponent<GameState>().increasePoints(enemyDodgePoints);
        }
    }

}

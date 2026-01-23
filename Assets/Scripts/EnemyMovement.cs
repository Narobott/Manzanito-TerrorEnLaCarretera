using UnityEngine;
using UnityEngine.Events;

public class EnemyMovement : MonoBehaviour
{
    private GameState gameState;

    public void SetGameState(GameState _gameState)
    {
        gameState = _gameState;
    }

    private bool bIsAlive = false;

    public UnityEvent<GameObject> EnemyDied;

    public void SetIsAlive(bool bAlive)
    {
        bIsAlive = bAlive;
        if (!bAlive )
        {
            gameObject.SetActive(false);
            EnemyDied.Invoke(gameObject);
        }
        else
        {
            gameObject.SetActive(true);
            gameObject.GetComponent<Collider2D>().enabled = true;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("DeadEnd"))
        {
            SetIsAlive(false);
        }
        if (collision.CompareTag("PointsEnd"))
        {
            GetComponent<Collider2D>().enabled = false;
        }
    }


    private void FixedUpdate()
    {
        if ( gameState.gameState == GameState.GameStateEnum.Game && bIsAlive)
        {
            gameObject.transform.position -= new Vector3(0, (float)0.1, 0);
        }
    }

}

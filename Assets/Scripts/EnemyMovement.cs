using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemyMovement : MonoBehaviour
{
    private GameState gameState;
    private float enemySpeed;
    [SerializeField] private GameObject PointsGO;
    [SerializeField] private GameObject ExplosionGO;

    public float getEnemySpeed()
    {
        return enemySpeed;
    }
    public void SetGameState(GameState _gameState)
    {
        gameState = _gameState;
    }


    private void Start()
    {
        RandomizeEnemySpeed();
    }

    public void RandomizeEnemySpeed(float maxSpeed = 0.75f)
    {
        enemySpeed = Random.Range(0.2f, maxSpeed);
    }

    private bool bIsAlive = false;

    public UnityEvent<GameObject> EnemyDied;

    public void SetIsAlive(bool bAlive)
    {
        bIsAlive = bAlive;
        if (!bAlive )
        {
            //gameObject.SetActive(false);
            EnemyDied.Invoke(gameObject);
        }
        else
        {
            //gameObject.SetActive(true);
            gameObject.GetComponent<Collider2D>().enabled = true;
            RandomizeEnemySpeed();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameState.gameState == GameState.GameStateEnum.Game)
        {
            if(collision.CompareTag("DeadEnd"))
            {
                SetIsAlive(false);
            }
            if (collision.CompareTag("PointsEnd"))
            {
                GetComponent<Collider2D>().enabled = false;
                StartCoroutine(WaitToEnableCollision());
                Instantiate(PointsGO, transform.position, Quaternion.identity);
                
                
            }
            if (collision.CompareTag("Character"))
            {
                collision.GetComponent<CharacterStats>().ReduceCharacterHealth();
                GameObject explosion = Instantiate(ExplosionGO, transform.position, Quaternion.identity);
                explosion.GetComponent<Animator>().SetTrigger("Explotar");
                StartCoroutine(DestruirESPLOSION(explosion));
            }

        }

    }

    private IEnumerator DestruirESPLOSION(GameObject ESPLOSION)
    {
        yield return new WaitForSeconds(.75f);
        GameObject.Destroy(ESPLOSION);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision");

    }


    private void FixedUpdate()
    {
        if ( gameState.gameState == GameState.GameStateEnum.Game && bIsAlive)
        {
            gameObject.transform.position -= new Vector3(0, enemySpeed, 0);
        }
    }

    private IEnumerator WaitToEnableCollision()
    {
        yield return new WaitForSeconds(1);
        GetComponent<Collider2D>().enabled = true;
    }

}

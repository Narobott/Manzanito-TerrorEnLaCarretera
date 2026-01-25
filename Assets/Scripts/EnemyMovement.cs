using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemyMovement : MonoBehaviour
{
    private GameState gameState;
    private float enemySpeed;
    [SerializeField] private GameObject PointsGO;
    [SerializeField] private GameObject ExplosionGO;
    private bool bNextPointIsCloseCall = false;


    [SerializeField] AudioClip closeCallAudio;


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
        enemySpeed = Random.Range(0.35f, maxSpeed);
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
            bNextPointIsCloseCall = false;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameState.gameState == GameState.GameStateEnum.Game)
        {
            if (collision.CompareTag("CloseCall"))
            {
                bNextPointIsCloseCall = true;
            }
            if (collision.CompareTag("Character"))
            {
                bNextPointIsCloseCall = false;
                if (collision.GetComponent<CharacterMovement>().bIsParryMode)
                {   
                    gameState.SetGameState(GameState.GameStateEnum.ImpactFrame);
                    explosionar();
                    GameObject points = Instantiate(PointsGO, transform.position, Quaternion.identity);
                    points.GetComponent<RetrievePoints>().SetIsParry(true);
                    SetIsAlive(false);

                }
                else
                {
                    collision.GetComponent<CharacterStats>().ReduceCharacterHealth();
                    explosionar();
                }
            }
            if (collision.CompareTag("PointsEnd"))
            {
                GetComponent<Collider2D>().enabled = false;
                StartCoroutine(WaitToEnableCollision());
                GameObject points = Instantiate(PointsGO, transform.position, Quaternion.identity);
                points.GetComponent<RetrievePoints>().SetIsCloseCall(bNextPointIsCloseCall);
                if (bNextPointIsCloseCall)
                {
                    GetComponent<AudioSource>().PlayOneShot(closeCallAudio);
                    GameObject.Find("Character").GetComponentInChildren<CharacterMovement>().SetCanParry(true);
                }

                bNextPointIsCloseCall = false;               
            }
            if(collision.CompareTag("DeadEnd"))
            {
                SetIsAlive(false);
            }

        }

    }
    private void explosionar()
    {
        GameObject explosion = Instantiate(ExplosionGO, transform.position, Quaternion.identity);
        explosion.GetComponent<Animator>().SetTrigger("Explotar");
        explosion.GetComponent<AudioSource>().Play();
        StartCoroutine(DestruirESPLOSION(explosion));
    }

    private IEnumerator DestruirESPLOSION(GameObject ESPLOSION)
    {
        yield return new WaitForSeconds(.6f);
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
        yield return new WaitForSeconds(0.3f);
        GetComponent<Collider2D>().enabled = true;
    }

}

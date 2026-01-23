using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] GameObject enemyPrefab;
    GameManager gameManager;
    GameState gameState;

    public UnityEvent enemySpawned;

    [SerializeField]
    private int InitialSpawnRate;
    [SerializeField]
    private float DificultyRateMultiplier;
    [SerializeField]
    private float minSpawnRate;

    private float SpawnRate;
    private float timeSinceLastSpawn;
    private int dificultyLevel;

    private List<GameObject> deadEnemies;
    

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        gameState = GetComponent<GameState>();
        deadEnemies = new List<GameObject>();
    }

    private void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject createdEnemy = Instantiate(enemyPrefab, new Vector3(100, 100, 100), Quaternion.identity);
            createdEnemy.GetComponent<EnemyMovement>().SetGameState(gameState);
            deadEnemies.Add(createdEnemy);
            
        }
    }

    private void FixedUpdate()
    {
        if (gameState.gameState == GameState.GameStateEnum.Game)
        {
            timeSinceLastSpawn += Time.deltaTime;

            dificultyLevel = Mathf.FloorToInt(gameState.GetGameTime() / 15f);

            SpawnRate = Mathf.Max(minSpawnRate, InitialSpawnRate - (DificultyRateMultiplier * dificultyLevel));

            if (timeSinceLastSpawn > SpawnRate)
            {
                SpawnEnemy();
                timeSinceLastSpawn = 0;
            }
        }

        
    }

    public void SpawnEnemy()
    {
        Vector3Int[] SpawnPoints = gameManager.GetHorizontalSnapPositions();
        int randomSpawnPointIndex = Random.Range(0, SpawnPoints.Length);
        Vector3 enemySpawnPosition = new(SpawnPoints[randomSpawnPointIndex].x, 11, 0);

        if (deadEnemies.Count < 1)
        {
            return;
        }

        GameObject enemy = deadEnemies[0];
        deadEnemies.Remove(enemy);

        enemy.transform.position = enemySpawnPosition;
        EnemyMovement enemyScript = enemy.GetComponent<EnemyMovement>();
        enemyScript.SetIsAlive(true);
        enemyScript.EnemyDied.RemoveListener(EnemyDied);
        enemyScript.EnemyDied.AddListener(EnemyDied);


        Debug.Log("Enemy spawned at position "+ enemySpawnPosition);
        enemySpawned.Invoke();
    }

private void EnemyDied(GameObject enemyDied)
    {
        deadEnemies.Add(enemyDied);
    }
}

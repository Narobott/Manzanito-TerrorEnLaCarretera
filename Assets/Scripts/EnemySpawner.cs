using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    private GameManager gameManager;
    private GameState gameState;

    public UnityEvent enemySpawned;

    [SerializeField] private int InitialSpawnRate;
    [SerializeField] private float DificultyRateMultiplier;
    [SerializeField] private float minSpawnRate;

    private float SpawnRate;
    private float timeSinceLastSpawn;
    private int dificultyLevel;

    private List<GameObject> deadEnemies;

    [SerializeField] private List<GameObject> enemiesOn0 = new();
    [SerializeField] private List<GameObject> enemiesOn1 = new();
    [SerializeField] private List<GameObject> enemiesOn2 = new();
    [SerializeField] private List<GameObject> enemiesOn3 = new();
    [SerializeField] private List<GameObject> enemiesOn4 = new();

    private List<GameObject>[] enemiesBySpawnPoint;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        gameState = GetComponent<GameState>();

        deadEnemies = new List<GameObject>();

        enemiesBySpawnPoint = new List<GameObject>[]
        {
            enemiesOn0,
            enemiesOn1,
            enemiesOn2,
            enemiesOn3,
            enemiesOn4
        };
    }

    private void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject createdEnemy = Instantiate(
                enemyPrefab,
                new Vector3(100, 100, 100),
                Quaternion.identity
            );

            createdEnemy.GetComponent<EnemyMovement>().SetGameState(gameState);
            createdEnemy.SetActive(false);
            deadEnemies.Add(createdEnemy);
        }
    }

    private void FixedUpdate()
    {
        if (gameState.gameState != GameState.GameStateEnum.Game)
            return;

        timeSinceLastSpawn += Time.deltaTime;

        dificultyLevel = Mathf.FloorToInt(gameState.GetGameTime() / 15f);
        SpawnRate = Mathf.Max(
            minSpawnRate,
            InitialSpawnRate - (DificultyRateMultiplier * dificultyLevel)
        );

        if (timeSinceLastSpawn > SpawnRate)
        {
            SpawnEnemy();
            timeSinceLastSpawn = 0f;
        }
    }

    public void SpawnEnemy()
    {
        if (deadEnemies.Count == 0)
            return;

        Vector3Int[] spawnPoints = gameManager.GetHorizontalSnapPositions();

        int spawnPointIndex;
        int playerFocusChance = Random.Range(0, 10);

        if (playerFocusChance > 6)
            spawnPointIndex = Random.Range(0, spawnPoints.Length);
        else
            spawnPointIndex = gameState.getPlayerPositionIndex();

        Vector3 enemySpawnPosition = new(
            spawnPoints[spawnPointIndex].x,
            11,
            0
        );

        GameObject enemy = deadEnemies[0];
        deadEnemies.RemoveAt(0);

        enemy.transform.position = enemySpawnPosition;

        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        enemyMovement.SetIsAlive(true);
        enemyMovement.EnemyDied.RemoveListener(EnemyDied);
        enemyMovement.EnemyDied.AddListener(EnemyDied);

        List<GameObject> enemiesOnPoint = enemiesBySpawnPoint[spawnPointIndex];

        // Speed limiting logic (applies to ALL spawn points)
        if (enemiesOnPoint.Count > 0)
        {
            EnemyMovement lastEnemyMovement =
                enemiesOnPoint[^1].GetComponent<EnemyMovement>();

            float speedOnPosition = lastEnemyMovement.getEnemySpeed();

            if (enemyMovement.getEnemySpeed() > speedOnPosition)
            {
                enemyMovement.RandomizeEnemySpeed(speedOnPosition);
            }
        }

        enemiesOnPoint.Add(enemy);

        Debug.Log("Enemy spawned at position " + spawnPointIndex);
        enemySpawned.Invoke();
    }

    private void EnemyDied(GameObject enemyDied)
    {
        deadEnemies.Add(enemyDied);

        for (int i = 0; i < enemiesBySpawnPoint.Length; i++)
        {
            bool bRemoved = enemiesBySpawnPoint[i].Remove(enemyDied);
            if (bRemoved) break;
        }
    }
}

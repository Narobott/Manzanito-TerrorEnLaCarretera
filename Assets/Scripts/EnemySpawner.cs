using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static GameState;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    private GameManager gameManager;
    private GameState gameState;

    public UnityEvent enemySpawned;

    public float GameTime;

    [SerializeField] private int InitialSpawnRate;
    [SerializeField] private float DificultyRateMultiplier;
    [SerializeField] private float minSpawnRate;

    private float SpawnRate;
    private float timeSinceLastSpawn;
    private int dificultyLevel;
    private int prevSpawnPosition = 6;
    private int timesSameSpawnPosition = 0;

    [SerializeField] AudioClip[] availableAudioClips;

    private List<GameObject> allEnemies = new List<GameObject>();
    private List<GameObject> deadEnemies = new List<GameObject>();

    private List<GameObject> enemiesOn0 = new();
    private List<GameObject> enemiesOn1 = new();
    private List<GameObject> enemiesOn2 = new();
    private List<GameObject> enemiesOn3 = new();
    private List<GameObject> enemiesOn4 = new();

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
        for (int i = 0; i < 15; i++)
        {
            GameObject createdEnemy = Instantiate(
                enemyPrefab,
                new Vector3(100, 100, 100),
                Quaternion.identity
            );

            createdEnemy.GetComponent<EnemyMovement>().SetGameState(gameState);
            //createdEnemy.SetActive(false);
            allEnemies.Add(createdEnemy);
            deadEnemies.Add(createdEnemy);
        }
    }

    private void FixedUpdate()
    {

        if (gameState.gameState == GameStateEnum.Game)
        {
            GameTime += Time.fixedDeltaTime;
        }

        if (gameState.gameState != GameState.GameStateEnum.Game)
            return;

        timeSinceLastSpawn += Time.fixedDeltaTime;

        dificultyLevel = Mathf.FloorToInt(GameTime / 15f);
        SpawnRate = Mathf.Max(
            minSpawnRate,
            InitialSpawnRate - (DificultyRateMultiplier * dificultyLevel)
        );

        if (timeSinceLastSpawn > SpawnRate)
        {
            SpawnEnemy();
            timeSinceLastSpawn = 0f;
        }

        gameState.GameStateText.text = "Current game state: " + gameState.gameState.ToString();
        gameState.SpawnRate.text = "Spawn rate: " + SpawnRate.ToString();
        gameState.TimeSinceLastSpawn.text = "Time since last spawn: " + timeSinceLastSpawn.ToString();
        gameState.DificultyLevel.text = "Dificulty level: " + GameTime.ToString() + " / 15 = " + dificultyLevel.ToString();
    }

    public void ResetDificultyLevel()
    {
        foreach (GameObject enemy in allEnemies)
        {
            enemy.transform.position = new Vector3(100, 100, 100);
            enemy.GetComponent<AudioSource>().Stop();
            EnemyDied(enemy);
        }
        SpawnRate = InitialSpawnRate;
        timeSinceLastSpawn = 0;
        dificultyLevel = 0;
    }

    public void SpawnEnemy()
    {
        if (deadEnemies.Count == 0)
            return;

        Vector3Int[] spawnPoints = gameManager.GetHorizontalSnapPositions();

        int spawnPointIndex;
        int playerFocusChance = UnityEngine.Random.Range(0, 10);

        if (playerFocusChance > 6)
            spawnPointIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        else
            spawnPointIndex = gameState.getPlayerPositionIndex();

        if (prevSpawnPosition == spawnPointIndex)
        {
            timesSameSpawnPosition++;

            for (int i = 0; i < timesSameSpawnPosition; i++)
            {
                playerFocusChance = UnityEngine.Random.Range(i, 10);
                if (playerFocusChance < 6)
                    spawnPointIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
                else
                    spawnPointIndex = gameState.getPlayerPositionIndex();
                if (spawnPointIndex != prevSpawnPosition)
                {
                    timesSameSpawnPosition = 0;
                    break;
                }
            }
        }

        prevSpawnPosition = spawnPointIndex;

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

        enemy.GetComponent<AudioSource>().generator = availableAudioClips[UnityEngine.Random.Range(0, availableAudioClips.Length)];
        enemy.GetComponent<AudioSource>().Play();

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

        enemyDied.transform.position = new Vector3(100, 100, 100);

    }
}

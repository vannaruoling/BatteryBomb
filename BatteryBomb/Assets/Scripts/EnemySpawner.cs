using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyTypes;
    public float spawnInterval = 2f;
    public Vector2 spawnPoint = new Vector2(-8f, 0f);

    // TODO: Edit these
    public GameObject bossPrefab; // null for no boss
    public int bossSpawnAfter = 4; // num enemies



    private float spawnTimer;
    private int enemiesToSpawn = 0;
    private int enemiesSpawnedCurrentWave = 0;
    private bool spawning = false;

    void Update()
    {
        if (!spawning) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            spawnTimer = spawnInterval;
            SpawnEnemy();
            enemiesToSpawn--;

            if (enemiesToSpawn <= 0)
            {
                spawning = false;
                // TODO: somehow broadcast end of round
            }
        }
    }

    public void SpawnWave(int count, float interval)
    {
        enemiesToSpawn = count;
        // TODO: slightly randomize the interval 
        spawnInterval = interval;
        spawnTimer = 0f;
        spawning = true;
        enemiesSpawnedCurrentWave = 0;
    }

    void SpawnEnemy()
    {
        GameObject enemyToSpawn;

        if (bossPrefab != null && enemiesSpawnedCurrentWave == bossSpawnAfter)
        {
            enemyToSpawn = bossPrefab;
        }
        else
        {
            enemyToSpawn = enemyTypes[Random.Range(0, enemyTypes.Length)];
        }

        Instantiate(enemyToSpawn, spawnPoint, Quaternion.identity);
        enemiesSpawnedCurrentWave++;

        // GameObject enemyToSpawn = enemyTypes[Random.Range(0, enemyTypes.Length)];
        // Instantiate(enemyToSpawn, spawnPoint, Quaternion.identity);
    }
}
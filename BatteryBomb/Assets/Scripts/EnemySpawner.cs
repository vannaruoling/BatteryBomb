using UnityEngine;



public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class WeightedEnemy
    {
        public GameObject prefab;
        public float baseWeight = 1f;
        public float weightPerWave = 0f;
    }

    public WeightedEnemy[] weightedEnemies;
    public float spawnInterval = 2f;
    public Vector2 spawnPoint = new Vector2(-8f, 0f);
    public Transform[] waypoints;

    // TODO: Edit these
    public GameObject bossPrefab; // null for no boss
    public int bossSpawnAfter = 4; // num enemies


    private float spawnTimer;
    private int enemiesToSpawn = 0;
    private int enemiesSpawnedCurrentWave = 0;
    private bool spawning = false;
    private int currentWave = 0;

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

    public void SpawnWave(int count, float interval, int waveNumber)
    {
        enemiesToSpawn = count;
        spawnInterval = interval;
        spawnTimer = 0f;
        spawning = true;
        enemiesSpawnedCurrentWave = 0;
        currentWave = waveNumber;
    }


    void SpawnEnemy()
    {
        GameObject enemyToSpawn;

        if (bossPrefab != null && enemiesSpawnedCurrentWave == bossSpawnAfter)
            enemyToSpawn = bossPrefab;
        else
            enemyToSpawn = PickWeightedEnemy();

        Vector3 spawnPos = (waypoints != null && waypoints.Length > 0 && waypoints[0] != null)
            ? waypoints[0].position
            : (Vector3)spawnPoint;

        GameObject spawned = Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);

        Enemy enemy = spawned.GetComponent<Enemy>();
        if (enemy != null) enemy.SetPath(waypoints);

        enemiesSpawnedCurrentWave++;
    }

    // void SpawnEnemy()
    // {
    //     GameObject enemyToSpawn;

    //     if (bossPrefab != null && enemiesSpawnedCurrentWave == bossSpawnAfter)
    //     {
    //         enemyToSpawn = bossPrefab;
    //     }
    //     else
    //     {
    //         enemyToSpawn = PickWeightedEnemy();
    //     }

    //     Instantiate(enemyToSpawn, spawnPoint, Quaternion.identity);
    //     enemiesSpawnedCurrentWave++;
    // }

    GameObject PickWeightedEnemy()
    {
        float totalWeight = 0f;
        foreach (var e in weightedEnemies)
        {
            totalWeight += Mathf.Max(0f, e.baseWeight + (e.weightPerWave * currentWave));
        }

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var e in weightedEnemies)
        {
            cumulative += Mathf.Max(0f, e.baseWeight + (e.weightPerWave * currentWave));
            if (roll <= cumulative) return e.prefab;
        }

        return weightedEnemies[weightedEnemies.Length - 1].prefab;
    }
}
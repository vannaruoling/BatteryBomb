using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public Vector2 spawnPoint = new Vector2(-8f, 0f);

    public float difficultyStep = 15f;
    public float minSpawnInterval = 0.5f;
    public float intervalDecreaseAmount = 0.2f;

    private float spawnTimer;
    private float difficultyGrowthTimer;

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            spawnTimer = spawnInterval;
            SpawnEnemy();
        }

        difficultyGrowthTimer -= Time.deltaTime;
        if (difficultyGrowthTimer <= 0f)
        {
            difficultyGrowthTimer = difficultyStep;
            spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - intervalDecreaseAmount);
        }
    }

    void SpawnEnemy()
    {
        Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
    }
}
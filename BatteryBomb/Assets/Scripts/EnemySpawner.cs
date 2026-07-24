using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyTypes;
    public float spawnInterval = 2f;
    public Vector2 spawnPoint = new Vector2(-8f, 0f);


    private float spawnTimer;
    private int enemiesToSpawn = 0;
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
    }

    void SpawnEnemy()
    {
        GameObject enemyToSpawn = enemyTypes[Random.Range(0, enemyTypes.Length)];
        Instantiate(enemyToSpawn, spawnPoint, Quaternion.identity);
    }
}
using UnityEngine;
using System.Collections.Generic;

public class BombSpawner : MonoBehaviour
{

    public GameObject bombPrefab;
    public float spawnInterval = 5f;
    // TODO: the amount in use shouldnt effect the max amount of bombs spawnnable
    public int maxUnattachedBombs = 4;

    public Vector2 spawnAreaMin = new Vector2(-6f, -3f);
    public Vector2 spawnAreaMax = new Vector2(6f, 3f);

    private float spawnTimer;
    private List<GameObject> activeBombs = new List<GameObject>();


    // Update is called once per frame
    void Update()
    {
        // Remove any null (detonated) bombs
        activeBombs.RemoveAll(bomb => bomb == null);

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            spawnTimer = spawnInterval;

            SpawnBombNow();
        }

    }

    public void SpawnBombNow()
    {
        if (CountUnattachedBombs() < maxUnattachedBombs)
        {
            SpawnBomb();
        }
    }


    int CountUnattachedBombs()
    {
        int count = 0;
        foreach (GameObject bomb in activeBombs)
        {
            if (bomb == null) continue;

            BatteryBomb bombScript = bomb.GetComponent<BatteryBomb>();
            if (bombScript != null && !bombScript.IsAttached)
            {
                count++;
            }
        }
        return count;
    }

    void SpawnBomb()
    {
        Vector2 spawnPos = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        GameObject bomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);
        activeBombs.Add(bomb);
    }
}

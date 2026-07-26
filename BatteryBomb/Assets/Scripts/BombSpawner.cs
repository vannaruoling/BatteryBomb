using UnityEngine;
using System.Collections.Generic;

public class BombSpawner : MonoBehaviour
{

    public GameObject bombPrefab;
    public CounterDisplay bombCounter;
    public float spawnInterval = 5f;
    // TODO: the amount in use shouldnt effect the max amount of bombs spawnnable
    public int baseBombsPerRound = 3;

    public Vector2 spawnAreaMin = new Vector2(-6f, -3f);
    public Vector2 spawnAreaMax = new Vector2(6f, 3f);

    private int bombsRemainingThisRound;

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
        if (bombsRemainingThisRound <= 0) return;


        SpawnBomb();
        bombsRemainingThisRound--;
        if (bombCounter != null) bombCounter.SetValue(bombsRemainingThisRound);
    }


    void SpawnBomb()
    {
        Vector2 spawnPos = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        GameObject bomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);
        activeBombs.Add(bomb);

        BatteryBomb bombScript = bomb.GetComponent<BatteryBomb>();
        if (bombScript != null) bombScript.DropIn(spawnPos);

        AudioManager.Instance.PlaySFX(AudioManager.Instance.bombSpawn, 0.5f);

    }

    public void ResetForRound()
    {
        bombsRemainingThisRound = baseBombsPerRound + UpgradeState.Instance.maxBombCountBonus;
        spawnTimer = spawnInterval;

        if (bombCounter != null) bombCounter.SetValue(bombsRemainingThisRound, true);
    }
}

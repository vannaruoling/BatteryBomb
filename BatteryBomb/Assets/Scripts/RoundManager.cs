using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    public EnemySpawner enemySpawner;
    public GameObject roundCardPanel;

    //TODO: change to like 100
    public int currentRound = 2;
    public int enemiesPerRound = 5;
    public float spawnInterval = 1.5f;

    private int enemiesAlive = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    public void StartRound()
    {
        roundCardPanel.SetActive(false);
        Time.timeScale = 1f;

        enemiesAlive = enemiesPerRound;
        enemySpawner.SpawnWave(enemiesPerRound, spawnInterval);
    }

    // Call this from Enemy.Die()
    public void ReportEnemyDeath()
    {
        enemiesAlive--;
        Debug.Log("Enemy died, remaining: " + enemiesAlive);
        if (enemiesAlive <= 0)
        {
            Debug.Log("Calling end round");
            EndRound();
        }
    }

    void EndRound()
    {
        // TODO: reconsider this mechanic
        ReviveAllTurrets();
        currentRound--;

        if (currentRound <= 0)
        {
            // TODO: win state, out of scope for now
            Debug.Log("All rounds cleared");
            return;
        }

        Debug.Log("Round end, pulling up round card panel");
        // StartRound();
        Time.timeScale = 0f;
        roundCardPanel.SetActive(true);
    }

    void ReviveAllTurrets()
    {
        Turret[] turrets = FindObjectsOfType<Turret>();
        foreach (Turret t in turrets)
        {
            t.Revive();
        }
    }
}
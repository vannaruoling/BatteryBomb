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
    private bool roundActive = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    public void StartRound()
    {
        roundCardPanel.SetActive(false);
        Time.timeScale = 1f;

        ResetGameBoard();
        GameManager.Instance.inputEnabled = true;

        // Display HP
        // TODO: Dont need this to fade in at the start of every round
        DamageFlashDisplay.Instance.ShowDamage(GameManager.Instance.playerHealth);

        enemiesAlive = enemiesPerRound;
        roundActive = true;
        enemySpawner.SpawnWave(enemiesPerRound, spawnInterval);
    }

    void ResetGameBoard()
    {
        Debug.Log("Resetting game board");
        BatteryBomb[] bombs = FindObjectsByType<BatteryBomb>(FindObjectsSortMode.None);
        foreach (BatteryBomb b in bombs)
        {
            Destroy(b.gameObject);
        }

        // Destroy all enemies to prevent them from persisting
        Debug.Log("Destroying all enemies");
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy e in enemies)
        {
            Destroy(e.gameObject);
        }

        TurretBase[] turrets = FindObjectsByType<TurretBase>(FindObjectsSortMode.None);
        foreach (TurretBase t in turrets)
        {
            t.Revive();
        }
    }

    // Call this from Enemy.Die()
    public void ReportEnemyDeath()
    {
        // PRevents bug where deaths were reported between rounds
        if (!roundActive) return;

        enemiesAlive--;
        Debug.Log("Enemy died, remaining: " + enemiesAlive);
        if (enemiesAlive <= 0)
        {
            roundActive = false;
            Debug.Log("Calling end round");
            EndRound();
        }
    }

    void EndRound()
    {
        GameManager.Instance.inputEnabled = false;

        currentRound--;

        if (currentRound <= 0)
        {
            // TODO: win state, out of scope for now
            Debug.Log("All rounds cleared");
            return;
        }

        Time.timeScale = 0f;
        roundCardPanel.SetActive(true);
        RoundCardManager.Instance.PresentRandomCards();
    }
}
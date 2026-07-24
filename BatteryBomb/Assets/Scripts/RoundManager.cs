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

        ResetGameBoard();
        GameManager.Instance.inputEnabled = true;


        DamageFlashDisplay.Instance.ShowDamage(GameManager.Instance.playerHealth);


        enemiesAlive = enemiesPerRound;
        enemySpawner.SpawnWave(enemiesPerRound, spawnInterval);
    }

    void ResetGameBoard()
    {
        BatteryBomb[] bombs = FindObjectsByType<BatteryBomb>(FindObjectsSortMode.None);
        foreach (BatteryBomb b in bombs)
        {
            Destroy(b.gameObject);
        }

        Turret[] basicTurrets = FindObjectsByType<Turret>(FindObjectsSortMode.None);
        foreach (Turret t in basicTurrets) t.Revive();

        // TODO: FindObjects cant search by interface
        TurretSpread[] spreadTurrets = FindObjectsByType<TurretSpread>(FindObjectsSortMode.None);
        foreach (TurretSpread t in spreadTurrets) t.Revive();
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
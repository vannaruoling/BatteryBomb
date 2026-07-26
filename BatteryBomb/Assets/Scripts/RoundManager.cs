using UnityEngine;
using System.Collections;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    public EnemySpawner enemySpawner;
    public BombSpawner bombSpawner;
    public GameObject roundCardPanel;
    public CounterDisplay roundCounter;
    public TurretPlacer turretPlacer;

    public int roundsPerPlacement = 1;


    //TODO: change to like 100
    public int currentRound = 2;
    public int baseEnemiesPerRound = 5;
    public float enemiesPerRoundGrowth = 0.6f;
    public float baseSpawnInterval = 1.5f;
    public float minSpawnInterval = 0.4f;

    public float spawnIntervalDecay = 0.03f;
    public int bossRoundInterval = 3;
    public float roundEndDelay = 0.4f;

    public CanvasGroup waveClearedBanner;
    public float bannerFadeInDuration = 0.15f;
    public float bannerHoldDuration = 0.6f;
    public float bannerFadeOutDuration = 0.2f;
    // Rounds per turret placement

    private int enemiesAlive = 0;
    private int wavesPlayed = 0;
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

        bombSpawner.ResetForRound();
        bombSpawner.SpawnBombNow();

        // Displays HP
        DamageFlashDisplay.Instance.ShowDamage(GameManager.Instance.playerHealth);
        if (roundCounter != null) roundCounter.SetValue(currentRound, true);

        // Calculate difficulty
        int scaledEnemies = baseEnemiesPerRound + Mathf.FloorToInt(wavesPlayed * enemiesPerRoundGrowth);
        float scaledInterval = Mathf.Max(minSpawnInterval, baseSpawnInterval - (wavesPlayed * spawnIntervalDecay));

        enemiesAlive = scaledEnemies;
        roundActive = true;
        wavesPlayed++;

        bool bossRound = (wavesPlayed % bossRoundInterval == 0);

        if (bossRound)
            AudioManager.Instance.PlayMusic(AudioManager.Instance.bossMusic);
        else
            AudioManager.Instance.PlayMusic(AudioManager.Instance.stageMusic);

        enemySpawner.SpawnWave(scaledEnemies, scaledInterval, wavesPlayed, bossRound);
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
            t.SetRangeIndicatorVisible(false);
            t.SetOutlineVisible(false);
        }
    }

    // Call this from Enemy.Die()
    public void ReportEnemyDeath()
    {
        // PRevents bug where deaths were reported between rounds
        if (!roundActive) return;

        GameManager.Instance.AddKill();

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

        if (roundCounter != null) roundCounter.SetValue(currentRound);

        if (currentRound <= 0)
        {
            UIManager.Instance.ShowWin();
            return;
        }

        // Lets player see last enemy death, feels less abrupt.
        StartCoroutine(EndRoundDelayed());
    }

    IEnumerator EndRoundDelayed()
    {
        yield return new WaitForSecondsRealtime(roundEndDelay);
        Time.timeScale = 0f;

        ClearBombs();

        if (waveClearedBanner != null)
            yield return StartCoroutine(ShowWaveClearedBanner());

        RequestNextRound();
    }

    IEnumerator ShowWaveClearedBanner()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.waveCleared);

        waveClearedBanner.gameObject.SetActive(true);
        waveClearedBanner.alpha = 0f;

        float t = 0f;
        while (t < bannerFadeInDuration)
        {
            t += Time.unscaledDeltaTime;
            waveClearedBanner.alpha = Mathf.Clamp01(t / bannerFadeInDuration);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(bannerHoldDuration);

        t = 0f;
        while (t < bannerFadeOutDuration)
        {
            t += Time.unscaledDeltaTime;
            waveClearedBanner.alpha = 1f - Mathf.Clamp01(t / bannerFadeOutDuration);
            yield return null;
        }
        waveClearedBanner.gameObject.SetActive(false);
    }

    // If getting a turret, no buff this round. Otherwise they always give buffs.
    public void RequestNextRound()
    {
        roundCardPanel.SetActive(true);

        if (wavesPlayed % roundsPerPlacement == 0)
            RoundCardManager.Instance.PresentTurretCards();
        else
            RoundCardManager.Instance.PresentRandomCards();
    }

    // Clears all bombs
    void ClearBombs()
    {
        BatteryBomb[] bombs = FindObjectsByType<BatteryBomb>(FindObjectsSortMode.None);
        foreach (BatteryBomb b in bombs)
        {
            Destroy(b.gameObject);
        }

        TurretBase[] turrets = FindObjectsByType<TurretBase>(FindObjectsSortMode.None);
        foreach (TurretBase t in turrets)
        {
            t.SetRangeIndicatorVisible(false);
            t.SetOutlineVisible(false);
        }
    }
}
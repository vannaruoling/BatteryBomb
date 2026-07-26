using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public bool skipTutorial = false;

    public static TutorialManager Instance;

    public TutorialDialogue dialogue;
    public CounterDisplay roundCounter;

    public GameObject professorPrefab;
    public Transform professorPoint;
    public GameObject bombPrefab;
    public GameObject enemyPrefab;
    public Transform[] enemyPoints;

    public Transform liveBombSpawnPoint;

    public float tutorialCountdown = 3f;
    public float tutorialExplosionRadius = 4f;

    private TurretBase professor;
    private BatteryBomb bomb;
    private System.Action onComplete;



    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    public void BeginTutorial(System.Action onCompleteCallback)
    {
        if (skipTutorial)
        {
            onCompleteCallback?.Invoke();
            return;
        }

        Debug.Log("Runing begin tutorial");
        onComplete = onCompleteCallback;
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        GameManager.Instance.inputEnabled = false;

        professor = Instantiate(professorPrefab, professorPoint.position, Quaternion.identity).GetComponent<TurretBase>();

        foreach (Transform p in enemyPoints)
        {
            Enemy e = Instantiate(enemyPrefab, p.position, Quaternion.identity).GetComponent<Enemy>();
            e.movementEnabled = false;
        }

        bomb = Instantiate(bombPrefab, liveBombSpawnPoint.position, Quaternion.identity).GetComponent<BatteryBomb>();
        bomb.countdownTime = tutorialCountdown;
        bomb.explosionRadius = tutorialExplosionRadius;

        yield return dialogue.Show("I've put my life's work into these Battery Bombs.");
        yield return dialogue.Show("It will give me the power to fight...at the cost of my life!");
        yield return dialogue.Show("Help attach that Battery Bomb onto me!");

        GameManager.Instance.inputEnabled = true;
        yield return new WaitUntil(() => bomb != null && bomb.AttachedTurret == professor);
        GameManager.Instance.inputEnabled = false;

        yield return dialogue.ShowAuto("BE GONE WRETCHED INSECTS!", 3f);

        yield return new WaitUntil(() => bomb == null);
        yield return new WaitForSeconds(2f);

        if (roundCounter != null)
        {
            roundCounter.gameObject.SetActive(true);
            roundCounter.SetValue(RoundManager.Instance.currentRound + 1, true);
            yield return new WaitForSeconds(0.6f);
            roundCounter.SetValue(RoundManager.Instance.currentRound);
            yield return new WaitForSeconds(0.8f);
        }

        professor.Revive();
        yield return dialogue.Show("Every new round brings us back. Now go.");

        Cleanup();

        System.Action cb = onComplete;
        onComplete = null;
        cb?.Invoke();
    }

    void Cleanup()
    {
        foreach (Enemy e in FindObjectsByType<Enemy>(FindObjectsSortMode.None)) Destroy(e.gameObject);
        foreach (BatteryBomb b in FindObjectsByType<BatteryBomb>(FindObjectsSortMode.None)) Destroy(b.gameObject);
        if (professor != null) Destroy(professor.gameObject);
    }
}
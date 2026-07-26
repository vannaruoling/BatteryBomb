using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject titlePanel;
    public GameObject gameplayPanel;
    public GameObject gameOverPanel;

    public GameObject winPanel;
    public float postTitleFadeDelay = 1f;
    public float titleFadeOutDuration = 0.4f;
    public CanvasGroup titleCanvasGroup;

    public CanvasGroup gameplayCanvasGroup;
    public float gameplayFadeInDuration = 0.4f;

    public GameObject titleScreenTileMap;
    public GameObject tutorialTileMap;
    public GameObject groundTileMap;

    private bool gameStarted = false;


    // Singleton
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ShowTitle();
    }

    void Start()
    {
        GameManager.Instance.inputEnabled = false;
    }

    public void ShowTitle()
    {
        SetActiveTileset(titleScreenTileMap);
        AudioManager.Instance.PlayMusic(AudioManager.Instance.titleMusic);

        Time.timeScale = 1f;
        titlePanel.SetActive(true);
        if (titleCanvasGroup != null) titleCanvasGroup.alpha = 1f;
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(false);

    }

    public void StartGame()
    {

        if (gameStarted) return;

        gameStarted = true;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonPress);
        StartCoroutine(StartGameSequence());
    }

    IEnumerator StartGameSequence()
    {
        if (titleCanvasGroup != null)
        {
            float t = 0f;
            while (t < titleFadeOutDuration)
            {
                t += Time.unscaledDeltaTime;
                titleCanvasGroup.alpha = 1f - Mathf.Clamp01(t / titleFadeOutDuration);
                yield return null;
            }
            titleCanvasGroup.alpha = 0f;
        }
        SetActiveTileset(tutorialTileMap);

        titlePanel.SetActive(false);
        gameplayPanel.SetActive(true);
        gameOverPanel.SetActive(false);

        if (gameplayCanvasGroup != null) gameplayCanvasGroup.alpha = 0f;

        Time.timeScale = 1f;
        GameManager.Instance.inputEnabled = false;

        if (gameplayCanvasGroup != null)
        {
            float t = 0f;
            while (t < gameplayFadeInDuration)
            {
                t += Time.unscaledDeltaTime;
                gameplayCanvasGroup.alpha = Mathf.Clamp01(t / gameplayFadeInDuration);
                yield return null;
            }
            gameplayCanvasGroup.alpha = 1f;
        }

        Debug.Log("starting tutorial..");
        TutorialManager.Instance.BeginTutorial(() => RoundManager.Instance.RequestNextRound());
    }

    public void ShowGameOver()
    {
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.gameOver);



        Time.timeScale = 0f;
        titlePanel.SetActive(false);
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(true);

        GameManager.Instance.inputEnabled = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowWin()
    {
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.victory);

        Time.timeScale = 0f;
        titlePanel.SetActive(false);
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        winPanel.SetActive(true);

        GameManager.Instance.inputEnabled = false;
    }

    public void SetActiveTileset(GameObject active)
    {
        Debug.Log("SetActiveTileset called with: " + (active == null ? "NULL" : active.name));

        if (titleScreenTileMap != null) titleScreenTileMap.SetActive(active == titleScreenTileMap);
        if (tutorialTileMap != null) tutorialTileMap.SetActive(active == tutorialTileMap);
        if (groundTileMap != null) groundTileMap.SetActive(active == groundTileMap);
    }
}
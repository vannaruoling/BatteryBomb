using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject titlePanel;
    public GameObject gameplayPanel;
    public GameObject gameOverPanel;

    public GameObject winPanel;


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
        AudioManager.Instance.PlayMusic(AudioManager.Instance.titleMusic);

        Time.timeScale = 1f;
        titlePanel.SetActive(true);
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(false);

    }

    public void StartGame()
    {
        titlePanel.SetActive(false);
        gameplayPanel.SetActive(true);
        gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
        GameManager.Instance.inputEnabled = false;

        Debug.Log("starting tutorial..");

        TutorialManager.Instance.BeginTutorial(() => RoundManager.Instance.RequestNextRound());

        // OLD VERSION
        // AudioManager.Instance.PlayMusic(AudioManager.Instance.stageMusic);

        // titlePanel.SetActive(false);
        // gameplayPanel.SetActive(true);
        // gameOverPanel.SetActive(false);

        // // Request next round should handle these next two calls in one now
        // // GameManager.Instance.inputEnabled = true;
        // // RoundManager.Instance.StartRound();
        // RoundManager.Instance.RequestNextRound();
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
}
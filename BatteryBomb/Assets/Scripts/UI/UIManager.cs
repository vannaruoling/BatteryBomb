using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject titlePanel;
    public GameObject gameplayPanel;
    public GameObject gameOverPanel;

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
        Time.timeScale = 0f;
        titlePanel.SetActive(true);
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(false);

    }

    public void StartGame()
    {
        titlePanel.SetActive(false);
        gameplayPanel.SetActive(true);
        gameOverPanel.SetActive(false);

        GameManager.Instance.inputEnabled = true;

        RoundManager.Instance.StartRound();
    }

    public void ShowGameOver()
    {
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
}
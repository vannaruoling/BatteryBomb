using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public bool inputEnabled = false;

    public int playerHealth = 10;
    // Score
    public int killCount = 0;

    private bool isGameOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void DamagePlayer(int amount)
    {
        if (isGameOver) return;

        playerHealth -= amount;
        Debug.Log("Player hurt: " + amount + ", curr health: " + playerHealth);

        DamageFlashDisplay.Instance.ShowDamage(playerHealth);

        if (playerHealth <= 0)
        {
            isGameOver = true;
            Debug.Log("YOU DIED");
            UIManager.Instance.ShowGameOver();
        }
    }

    public void AddKill()
    {
        killCount++;
    }
    public void HealPlayer(int amount)
    {
        playerHealth = Mathf.Min(playerHealth + amount, 10);
    }

    // // Update is called once per frame
    // void Update()
    // {

    // }
}

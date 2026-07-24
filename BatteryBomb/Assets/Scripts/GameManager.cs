using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public bool inputEnabled = false;

    public int playerHealth = 10;
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
        playerHealth -= amount;
        Debug.Log("Player hurt: " + amount + ", curr health: " + playerHealth);

        if (playerHealth <= 0)
        {
            Debug.Log("YOU ARE DEAD");
            UIManager.Instance.ShowGameOver();
        }
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

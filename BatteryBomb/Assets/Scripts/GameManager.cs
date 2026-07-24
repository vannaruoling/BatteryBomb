using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

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

    // // Update is called once per frame
    // void Update()
    // {

    // }
}

using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 3;
    public float speed = 2f;
    public bool movementEnabled = true;
    private int currentHealth;

    // Used to avoid null reference from race condition
    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (movementEnabled)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime, Space.World);
        }

    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Enemy hurt: " + amount + ", curr health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        RoundManager.Instance.ReportEnemyDeath();
        Debug.Log("Enemy died");
        Destroy(gameObject);
    }

    // // Update is called once per frame
    // void Update()
    // {

    // }
}

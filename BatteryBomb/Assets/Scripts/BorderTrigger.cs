using UnityEngine;

public class BorderTrigger : MonoBehaviour
{
    public int damageToPlayer = 1;

    // When enemy collides, player damage
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameManager.Instance.DamagePlayer(damageToPlayer);
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(int.MaxValue);
            }
        }
    }
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {

    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }
}

using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public float maxLifetime = 3f;

    private Vector2 direction;
    private bool hasTarget = false;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        hasTarget = true;
    }

    void Update()
    {
        // Move towards target
        if (hasTarget)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }

        // Check lifetime
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Tapesafe here is the script name rather than the tag name
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Method call from Enemy object
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
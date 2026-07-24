using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject explosionEffect;
    public float explosionScale = 0.5f;
    public float speed = 10f;
    public int damage = 1;
    public float maxLifetime = 3f;

    public bool isAOE = false;
    public float aoeRadius = 1.5f;

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
            if (isAOE)
            {
                AOEDamage();
            }
            else
            {
                // Typesafe here is script name rather than tag
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }

            Destroy(gameObject);
        }
    }

    void AOEDamage()
    {
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            explosion.transform.localScale *= explosionScale;
            Destroy(explosion, 0.5f);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aoeRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }
    }


}
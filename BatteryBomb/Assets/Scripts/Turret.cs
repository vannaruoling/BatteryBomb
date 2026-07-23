using UnityEngine;

public class Turret : MonoBehaviour
{
    public float range = 5f;
    public float fireRate = 1f;
    public GameObject projectilePrefab;

    private float fireCooldown = 0f;
    private Transform target;

    void Start()
    {
        // Initialization
    }

    void Update()
    {
        FindTarget();

        if (target != null)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = 1f / fireRate;
            }
        }
    }

    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        // Find closest enemy in radius
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance && distance <= range)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        target = closestEnemy;
    }

    void Shoot()
    {
        GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        Vector2 dir = (target.position - transform.position);
        projectile.SetDirection(dir);
    }
}




using UnityEngine;

public class Turret : MonoBehaviour
{
    public float range = 5f;
    public float fireRate = 1f;
    public GameObject projectilePrefab;

    public bool isPowered = false;
    public bool isDead = false;
    private float fireCooldown = 0f;
    private Transform target;

    void Start()
    {
        SetPowered(false);
        // Initialization
    }

    void Update()
    {
        if (!isPowered || isDead)
        {
            return;
        }

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

    public void SetPowered(bool powered)
    {
        if (isDead) return;
        isPowered = powered;
        GetComponent<SpriteRenderer>().color = powered ? Color.green : Color.gray;
    }


    public void Die()
    {
        isDead = true;
        SetPowered(false);
        GetComponent<SpriteRenderer>().color = Color.black;
    }

    // TODO:
    public void Revive()
    {
        isDead = false;
        SetPowered(false);
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
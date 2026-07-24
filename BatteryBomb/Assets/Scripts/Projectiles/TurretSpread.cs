using UnityEngine;

public class TurretSpread : MonoBehaviour, ITurret
{
    public float fireRate = 1f;
    public int projectileCount = 6;
    public GameObject projectilePrefab;

    public bool isPowered = false;
    public bool isDead = false;

    public bool IsPowered => isPowered;
    public bool IsDead => isDead;

    private float fireCooldown = 0f;

    void Update()
    {
        if (!isPowered || isDead) return;

        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            FireBurst();
            fireCooldown = 1f / fireRate;
        }
    }

    void FireBurst()
    {
        float angleStep = 360f / projectileCount;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = i * angleStep;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.SetDirection(dir);
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

    public void Revive()
    {
        isDead = false;
        SetPowered(false);
    }
}
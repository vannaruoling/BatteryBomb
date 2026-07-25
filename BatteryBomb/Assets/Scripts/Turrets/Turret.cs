using UnityEngine;

public class Turret : TurretBase
{
    private Transform target;
    private Vector2 dir; 

    void Update()
    {
        // Call the base class Update so firing cooldowns and outlines work!
        base.Update(); 

        // If the turret is offline or dead, don't track targets
        if (!isPowered || isDead) return;

        FindTarget();
        
        if (target != null)
        {
            dir = (target.position - transform.position);
            RotateTowardsTarget();
        }
    }

    protected override bool TryFire()
    {
        if (target == null) return false;

        GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        
        projectile.SetDirection(dir);
        return true;
    }

    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

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

    // Removed 'new' because TurretBase doesn't actually have this method
    void RotateTowardsTarget() 
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        // Adjust the -90f offset depending on which way your sprite faces by default
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 270f);
    }
}

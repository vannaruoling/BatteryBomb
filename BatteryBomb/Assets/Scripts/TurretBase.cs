using UnityEngine;

public abstract class TurretBase : MonoBehaviour
{
    public float fireRate = 1f;
    public GameObject projectilePrefab;

    public bool isPowered = false;
    public bool isDead = false;

    protected float fireCooldown = 0f;

    protected virtual void Start()
    {
        SetPowered(false);
    }

    protected virtual void Update()
    {
        if (!isPowered || isDead) return;

        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            if (TryFire())
            {
                fireCooldown = 1f / fireRate;
            }
        }
    }

    // return false if the turret doesnt fire
    // avoids losing cooldown 
    protected abstract bool TryFire();

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
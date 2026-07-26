using UnityEngine;
using System.Collections;


public enum TurretType { Basic, Spread, Cannon }
public abstract class TurretBase : MonoBehaviour
{
    public float fireRate = 1f;
    public GameObject projectilePrefab;
    public TurretType turretType;
    public bool isPowered = false;
    public bool isDead = false;
    public float range = 5f;

    public float shootPitch = 1f;

    public Color reviveFlashColor = Color.green;
    public float reviveFlashDuration = 0.3f;
    public float revivePulseScale = 1.4f;
    public float revivePulseDuration = 0.35f;
    private Vector3 baseTurretScale;

    public BatteryBomb attachedBomb;
    public GameObject rangeIndicatorPrefab;
    public Material outlineMaterial;

    public float reattachCooldown = 0.2f;
    private float lastDetachTime = -999f;
    private SpriteRenderer outlineRenderer;
    private SpriteRenderer spriteRenderer;
    private GameObject rangeIndicatorInstance;
    private Animator animator;



    protected float fireCooldown = 0f;

    protected virtual void Start()
    {
        baseTurretScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        SetPowered(false);

        UpgradeState.TurretUpgrade upg = UpgradeState.Instance.GetUpgrade(turretType);
        fireRate *= upg.fireRateMultiplier;
        range += upg.rangeBonus;

        if (rangeIndicatorPrefab != null)
        {
            rangeIndicatorInstance = Instantiate(rangeIndicatorPrefab, transform);
            rangeIndicatorInstance.transform.localPosition = Vector3.zero;
            UpdateRangeIndicatorScale();
            rangeIndicatorInstance.SetActive(false);
        }

        outlineRenderer = OutlineUtility.CreateOutline(transform, spriteRenderer, outlineMaterial);
    }

    protected virtual void Update()
    {
        // keep outline matched to the current animation frame
        if (outlineRenderer != null && outlineRenderer.enabled)
        {
            outlineRenderer.sprite = spriteRenderer.sprite;
        }

        if (!isPowered || isDead) return;

        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            if (TryFire())
            {
                fireCooldown = 1f / fireRate;
                AudioManager.Instance.PlaySFX(AudioManager.Instance.turretShoot, 0.25f, shootPitch);
            }
        }
    }

    public void SetOutlineVisible(bool show)
    {
        if (outlineRenderer != null)
        {
            outlineRenderer.gameObject.SetActive(show);
            if (show) outlineRenderer.sprite = spriteRenderer.sprite;
        }
    }

    // return false if the turret doesnt fire
    // avoids losing cooldown 
    protected abstract bool TryFire();

    public void SetPowered(bool powered)
    {
        if (isDead) return;
        isPowered = powered;
        if (animator != null)
        {
            animator.Play(powered ? "shooting" : "stationary");
        }
        if (powered)
        {
            Juice.Instance.ShakeTransform(transform, 0.06f, 0.12f);
            Juice.Instance.FlashSprite(GetComponent<SpriteRenderer>(), Color.white, 0.1f);
        }
    }
    // public void SetPowered(bool powered)
    // {
    //     if (isDead) return;
    //     isPowered = powered;
    //     if (animator != null)
    //     {
    //         animator.Play(powered ? "shooting" : "stationary");
    //     }
    // }

    public void Die()
    {
        isDead = true;
        if (animator != null)
        {
            animator.Play("dead");
        }
    }

    public void Revive()
    {
        bool wasDead = isDead;
        isDead = false;
        SetPowered(false);

        if (wasDead)
        {
            if (spriteRenderer != null) Juice.Instance.FlashSprite(spriteRenderer, reviveFlashColor, reviveFlashDuration);
            StartCoroutine(RevivePulseRoutine());
        }
    }

    // TODO: when player clicks a turret, display its range
    void UpdateRangeIndicatorScale()
    {
        if (rangeIndicatorInstance == null) return;

        SpriteRenderer sr = rangeIndicatorInstance.GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;

        float nativeDiameter = sr.sprite.bounds.size.x;
        float desiredDiameter = range * 2f;

        // ignore parent scale
        float scaleFactor = desiredDiameter / nativeDiameter / transform.localScale.x;

        rangeIndicatorInstance.transform.localScale = Vector3.one * scaleFactor;
    }

    public void SetRangeIndicatorVisible(bool show)
    {
        if (rangeIndicatorInstance != null)
        {
            if (show) UpdateRangeIndicatorScale();
            rangeIndicatorInstance.SetActive(show);
        }
    }

    public bool CanAttach()
    {
        return Time.time - lastDetachTime >= reattachCooldown;
    }

    public void NotifyDetached()
    {
        lastDetachTime = Time.time;
    }

    IEnumerator RevivePulseRoutine()
    {
        float t = 0f;
        while (t < revivePulseDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / revivePulseDuration);

            // overshoot up then settle, like a squish-bounce
            float eased = Mathf.Sin(p * Mathf.PI); // rises then falls back to 1
            float scale = Mathf.Lerp(1f, revivePulseScale, eased);

            transform.localScale = baseTurretScale * scale;
            yield return null;
        }
        transform.localScale = baseTurretScale;
    }
}
using UnityEngine;

public abstract class TurretBase : MonoBehaviour
{
    public float fireRate = 1f;
    public GameObject projectilePrefab;

    public bool isPowered = false;
    public bool isDead = false;
    public float range = 5f;
    public GameObject rangeIndicatorPrefab;
    private GameObject rangeIndicatorInstance;
    private SpriteRenderer outlineRenderer;

    protected float fireCooldown = 0f;

    protected virtual void Start()
    {
        SetPowered(false);

        if (rangeIndicatorPrefab != null)
        {
            rangeIndicatorInstance = Instantiate(rangeIndicatorPrefab, transform);
            rangeIndicatorInstance.transform.localPosition = Vector3.zero;
            UpdateRangeIndicatorScale();
            rangeIndicatorInstance.SetActive(false);
        }

        float outlineScale = 1.3f;
        outlineRenderer = OutlineUtility.CreateOutline(transform, GetComponent<SpriteRenderer>(), outlineScale);

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

    public void SetOutlineVisible(bool show)
    {
        if (outlineRenderer != null)
        {
            outlineRenderer.gameObject.SetActive(show);
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
}
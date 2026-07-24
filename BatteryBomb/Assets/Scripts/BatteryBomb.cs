using UnityEngine;
using TMPro;

public class BatteryBomb : MonoBehaviour
{

    public float countdownTime = 10f;
    public TextMeshProUGUI countdownText;
    public float attachRadius = 1f;
    public GameObject explosionEffect;
    public float explosionRadius = 2f;
    public int explosionDamage = 3;
    public bool IsAttached => attachedTurret != null;

    private TurretBase highlightedTurret = null;
    private bool isDragging = false;
    private Camera mainCamera;
    private TurretBase attachedTurret = null;

    void Awake()
    {
        mainCamera = Camera.main;
        // TODO: imrpvoe the code for upgrades so its not hardcoded like this
        countdownTime += UpgradeState.Instance.bombTimerBonus;
        explosionRadius += UpgradeState.Instance.explosionRadiusBonus;
        SetPowering();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCountdownDisplay();

        if (!GameManager.Instance.inputEnabled) return;

        if (isDragging)
        {
            // Track mouse position
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            transform.position = mouseWorldPos;

            // Get highlighted turret range
            TurretBase target = FindAttachableTurret();
            if (target != highlightedTurret)
            {
                if (highlightedTurret != null) highlightedTurret.SetRangeIndicatorVisible(false);
                if (target != null) target.SetRangeIndicatorVisible(true);
                highlightedTurret = target;
            }
        }

        if (attachedTurret != null)
        {
            countdownTime -= Time.deltaTime;
            if (countdownTime <= 0f)
            {
                Detonate();
            }
        }

        UpdateCountdownDisplay();
    }

    void UpdateCountdownDisplay()
    {
        if (countdownText != null)
        {
            // round up so that 0 is gonezo
            countdownText.text = Mathf.CeilToInt(countdownTime).ToString();
        }
    }

    public void SetPowering()
    {
        bool isPowered = attachedTurret != null;
        GetComponent<SpriteRenderer>().color = isPowered ? Color.red : Color.yellow;
    }

    void OnMouseDown()
    {
        if (!GameManager.Instance.inputEnabled) return;

        if (attachedTurret != null)
        {
            Detach();
        }

        isDragging = true;
    }

    void OnMouseUp()
    {
        if (!GameManager.Instance.inputEnabled) return;
        isDragging = false;

        if (highlightedTurret != null)
        {
            highlightedTurret.SetRangeIndicatorVisible(false);
            highlightedTurret = null;
        }

        Attach();
    }

    void Detach()
    {
        attachedTurret.SetPowered(false);
        attachedTurret = null;
        SetPowering();
    }

    void Attach()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attachRadius, LayerMask.GetMask("Default"));

        foreach (Collider2D hit in hits)
        {
            TurretBase turret = hit.GetComponent<TurretBase>();
            if (turret != null && !turret.isDead && !turret.isPowered)
            {
                Debug.Log("Found turret to attach to: " + hit.gameObject.name);
                attachedTurret = turret;
                transform.position = turret.transform.position + new Vector3(0f, 0.5f, 0f);
                attachedTurret.SetPowered(true);
                SetPowering();
                return;
            }
        }

        Debug.Log("No turret found to attach to");
    }


    void Detonate()
    {
        Debug.Log("Battery BOOOOOMMMBBB");

        Vector3 explosionPosition = attachedTurret.transform.position;

        GameObject explosion = Instantiate(explosionEffect, explosionPosition, Quaternion.identity);
        Destroy(explosion, 1f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius, LayerMask.GetMask("Default"));

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(explosionDamage);
                }
            }
        }

        attachedTurret.Die();
        Destroy(gameObject);
    }

    TurretBase FindAttachableTurret()
    {
        // FInd turret on defualt layer
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attachRadius, LayerMask.GetMask("Default"));

        foreach (Collider2D hit in hits)
        {
            TurretBase turret = hit.GetComponent<TurretBase>();
            if (turret != null && !turret.isDead && !turret.isPowered)
            {
                return turret;
            }
        }

        return null;
    }
}

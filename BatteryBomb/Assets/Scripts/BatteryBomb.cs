using UnityEngine;
using TMPro;
using System.Collections;

public class BatteryBomb : MonoBehaviour
{

    public float countdownTime = 10f;
    public TextMeshProUGUI countdownText;
    public float attachRadius = 1f;
    public GameObject explosionEffect;
    public float explosionRadius = 2f;
    public int explosionDamage = 3;
    public bool IsAttached => attachedTurret != null;
    public Material outlineMaterial;



    // Visual countdown tuning
    public float tickShakeMagnitude = 0.05f;
    public float tickShakeDuration = 0.15f;
    public Color tickFlashColor = Color.white;
    public float tickFlashDuration = 0.1f;


    // Booting animation for when a bomb is swapped out
    public float puntDuration = 0.25f;
    public float puntHeight = 0.3f;
    private bool isPunting = false;
    private int lastDisplayedSecond = -1;



    private Coroutine puntRoutine;
    private SpriteRenderer outlineRenderer;


    private TurretBase highlightedTurret = null;
    private SpriteRenderer spriteRenderer;
    private bool isDragging = false;
    private Camera mainCamera;
    private TurretBase attachedTurret = null;
    private static bool anyBombDragging = false;

    void Awake()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 20;

        // TODO: imrpvoe the code for upgrades so its not hardcoded like this
        countdownTime += UpgradeState.Instance.bombTimerBonus;
        explosionRadius += UpgradeState.Instance.explosionRadiusBonus;
        SetPowering();

        lastDisplayedSecond = Mathf.CeilToInt(countdownTime);

        // Selector outline
        outlineRenderer = OutlineUtility.CreateOutline(transform, spriteRenderer, outlineMaterial);
        if (outlineRenderer != null) outlineRenderer.gameObject.SetActive(false);
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
                if (highlightedTurret != null)
                {
                    highlightedTurret.SetRangeIndicatorVisible(false);
                    highlightedTurret.SetOutlineVisible(false);
                }
                if (target != null)
                {
                    target.SetRangeIndicatorVisible(true);
                    target.SetOutlineVisible(true);
                }
                highlightedTurret = target;
            }
        }

        if (attachedTurret != null)
        {
            countdownTime -= Time.deltaTime;
            int currentSecond = Mathf.CeilToInt(countdownTime);
            if (currentSecond != lastDisplayedSecond)
            {
                lastDisplayedSecond = currentSecond;
                Juice.Instance.ShakeTransform(transform, tickShakeMagnitude, tickShakeDuration);
                Juice.Instance.FlashSprite(spriteRenderer, tickFlashColor, tickFlashDuration);
            }


            if (countdownTime <= 0f)
            {
                Detonate();
            }
        }

        UpdateCountdownDisplay();
    }

    void LateUpdate()
    {
        if (outlineRenderer != null && outlineRenderer.gameObject.activeSelf && !isDragging)
        {
            outlineRenderer.sprite = spriteRenderer.sprite;
        }
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
        GetComponent<SpriteRenderer>().color = isPowered ? Color.Lerp(Color.white, Color.red, 0.3f) : Color.yellow;
    }

    void Drop()
    {
        isDragging = false;
        anyBombDragging = false;
        spriteRenderer.sortingOrder = 20;

        if (highlightedTurret != null)
        {
            highlightedTurret.SetRangeIndicatorVisible(false);
            highlightedTurret.SetOutlineVisible(false);
            highlightedTurret = null;
        }

        Attach();
    }
    void OnMouseUp()
    {
        if (!GameManager.Instance.inputEnabled) return;
        Drop();
    }

    void OnMouseDown()
    {
        if (!GameManager.Instance.inputEnabled) return;

        if (isPunting)
        {
            StopCoroutine(puntRoutine);
            isPunting = false;
        }

        if (attachedTurret != null)
        {
            Detach();
        }

        isDragging = true;
        anyBombDragging = true;
        spriteRenderer.sortingOrder = 5;
        SetBombOutlineVisible(false);
    }

    public void Detach(bool punted = false)
    {
        if (attachedTurret.attachedBomb == this) attachedTurret.attachedBomb = null;

        Vector3 restPos = attachedTurret.transform.position + new Vector3(0f, -0.6f, 0f);

        attachedTurret.SetPowered(false);
        attachedTurret = null;
        SetPowering();

        if (punted)
        {
            puntRoutine = StartCoroutine(PuntRoutine(restPos));
        }
        else
        {
            transform.position = restPos;
        }
    }

    IEnumerator PuntRoutine(Vector3 restPos)
    {
        isPunting = true;
        Vector3 start = transform.position;
        float sideDir = Random.value > 0.5f ? 1f : -1f;

        float t = 0f;
        while (t < puntDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / puntDuration);

            Vector3 pos = Vector3.Lerp(start, restPos, p);
            pos.y += Mathf.Sin(p * Mathf.PI) * puntHeight; // little hop
            pos.x += sideDir * 0.15f * (1f - p);            // drifts sideways, settles

            transform.position = pos;
            yield return null;
        }

        transform.position = restPos;
        isPunting = false;
    }

    void Attach()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attachRadius, LayerMask.GetMask("Default"));

        foreach (Collider2D hit in hits)
        {
            TurretBase turret = hit.GetComponent<TurretBase>();
            if (turret != null && !turret.isDead)
            {
                if (turret.attachedBomb != null && turret.attachedBomb != this)
                {
                    turret.attachedBomb.Detach(true);
                }

                attachedTurret = turret;
                transform.position = turret.transform.position + new Vector3(0f, 0.5f, 0f);
                attachedTurret.SetPowered(true);
                attachedTurret.attachedBomb = this;
                SetPowering();
                // SetBombOutlineVisible(false);
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attachRadius, LayerMask.GetMask("Default"));

        foreach (Collider2D hit in hits)
        {
            TurretBase turret = hit.GetComponent<TurretBase>();
            if (turret != null && !turret.isDead)
            {
                return turret;
            }
        }

        return null;
    }

    void OnDestroy()
    {
        if (highlightedTurret != null)
        {
            highlightedTurret.SetRangeIndicatorVisible(false);
            highlightedTurret.SetOutlineVisible(false);
            highlightedTurret = null;
        }
    }

    void OnMouseEnter()
    {

        if (!GameManager.Instance.inputEnabled) return;
        if (isDragging) return;
        if (anyBombDragging) return;
        SetBombOutlineVisible(true);
        // if (!GameManager.Instance.inputEnabled) return;
        // if (isDragging) return;
        // if (attachedTurret != null) return;
        // SetBombOutlineVisible(true);
    }


    void OnMouseExit()
    {
        if (isDragging) return;
        SetBombOutlineVisible(false);
    }
    void SetBombOutlineVisible(bool show)
    {
        if (outlineRenderer != null)
        {
            outlineRenderer.gameObject.SetActive(show);
            if (show) outlineRenderer.sprite = spriteRenderer.sprite;
        }
    }
}

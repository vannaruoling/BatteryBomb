using UnityEngine;
using TMPro;
using System.Collections;

public class BatteryBomb : MonoBehaviour
{
    public RuntimeAnimatorController unpoweredController;
    public RuntimeAnimatorController poweredController;
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

    // TUTORIAL PURPOSSESS
    public bool isInert = false;
    public TurretBase AttachedTurret => attachedTurret;

    private bool hasBeenAttached = false;
    private bool isPunting = false;

    public float attachPunchScale = 1.3f;
    public float attachPunchDuration = 0.15f;
    public Color attachFlashColor = Color.white;
    public float attachFlashDuration = 0.12f;
    private Vector3 baseScale;

    private int lastDisplayedSecond = -1;
    private Animator animator;



    private const float zOffset = -0.5f;
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
        animator = GetComponent<Animator>();
        baseScale = transform.localScale;
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

        if (GameManager.Instance.inputEnabled)
        {
            if (isDragging)
            {
                // Track mouse position
                Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = zOffset;
                mouseWorldPos = ScreenBounds.ClampToCameraView(mouseWorldPos, mainCamera, 1f); // 32px pad
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
        }

        if (attachedTurret != null && !isInert)
        {
            countdownTime -= Time.deltaTime;
            int currentSecond = Mathf.CeilToInt(countdownTime);
            if (currentSecond != lastDisplayedSecond)
            {
                // TODO: this might be super annoying
                AudioManager.Instance.PlaySFX(AudioManager.Instance.bombTick, 0.5f);

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
            bool shouldShow = hasBeenAttached && !isInert;
            countdownText.gameObject.SetActive(shouldShow);

            if (shouldShow)
            {
                countdownText.text = Mathf.CeilToInt(countdownTime).ToString();
            }
        }
    }

    public void SetPowering()
    {
        if (isInert)
        {
            GetComponent<SpriteRenderer>().color = Color.gray;
            if (animator != null && unpoweredController != null) animator.runtimeAnimatorController = unpoweredController;
            return;
        }
        bool isPowered = attachedTurret != null;
        GetComponent<SpriteRenderer>().color = isPowered ? Color.Lerp(Color.white, Color.red, 0.3f) : Color.yellow;

        if (animator != null)
        {
            RuntimeAnimatorController target = isPowered ? poweredController : unpoweredController;
            if (target != null) animator.runtimeAnimatorController = target;
        }
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

        AudioManager.Instance.PlaySFX(AudioManager.Instance.bombPickup);

        isDragging = true;
        anyBombDragging = true;
        spriteRenderer.sortingOrder = 5;
        SetBombOutlineVisible(false);
    }

    public void Detach(bool punted = false)
    {
        if (attachedTurret.attachedBomb == this) attachedTurret.attachedBomb = null;

        attachedTurret.NotifyDetached();

        Vector3 restPos = attachedTurret.transform.position + new Vector3(0f, -0.6f, 0f);
        restPos.z = zOffset;

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

    IEnumerator AttachPunchRoutine()
    {
        float t = 0f;
        while (t < attachPunchDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / attachPunchDuration);
            float scale = Mathf.Lerp(attachPunchScale, 1f, p);
            transform.localScale = baseScale * scale;
            yield return null;
        }
        transform.localScale = baseScale;
    }

    void Attach()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attachRadius, LayerMask.GetMask("Default"));

        foreach (Collider2D hit in hits)
        {
            TurretBase turret = hit.GetComponent<TurretBase>();
            if (turret != null && !turret.isDead && turret.CanAttach())
            {
                if (turret.attachedBomb != null && turret.attachedBomb != this)
                {
                    turret.attachedBomb.Detach(true);
                }

                attachedTurret = turret;
                hasBeenAttached = true;

                Vector3 snapPos = turret.transform.position + new Vector3(0f, 0.5f, 0f);
                snapPos.z = zOffset;
                transform.position = snapPos;

                AudioManager.Instance.PlaySFX(AudioManager.Instance.bombAttach);

                attachedTurret.SetPowered(true);
                attachedTurret.attachedBomb = this;
                SetPowering();
                Juice.Instance.FlashSprite(spriteRenderer, attachFlashColor, attachFlashDuration);
                StartCoroutine(AttachPunchRoutine());
                // SetBombOutlineVisible(false);
                return;
            }
        }

        Debug.Log("No turret found to attach to");
    }


    void Detonate()
    {
        Debug.Log("Battery BOOOOOMMMBBB");
        Debug.Log("Detonate() called on " + gameObject.name + " at frame " + Time.frameCount);


        Vector3 explosionPosition = attachedTurret.transform.position;


        GameObject explosion = Instantiate(explosionEffect, explosionPosition, Quaternion.identity);
        SpriteRenderer explosionSprite = explosion.GetComponentInChildren<SpriteRenderer>();
        if (explosionSprite != null)
        {
            Color fadeColor = explosionSprite.color;
            fadeColor.a = 0f;
            Juice.Instance.FadeSpriteToColor(explosionSprite, fadeColor, 1f, () => Destroy(explosion));
        }
        else
        {
            Destroy(explosion, 1.5f);
        }

        AudioManager.Instance.PlaySFX(AudioManager.Instance.bombExplode);

        Collider2D[] hits = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius, LayerMask.GetMask("Default"));

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeChainDamage(explosionDamage);

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
            if (turret != null && !turret.isDead && turret.CanAttach())
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


    // TUTORIAL PURPOSES
    public void TutorialAttachTo(TurretBase turret, bool powerTurret)
    {
        attachedTurret = turret;
        hasBeenAttached = true;
        turret.attachedBomb = this;
        Vector3 pos = turret.transform.position + new Vector3(0f, 0.5f, 0f);
        pos.z = zOffset;
        transform.position = pos;
        if (powerTurret) turret.SetPowered(true);
        SetPowering();
    }

    // TUTORIAL PURPOSES
    public void SetLive()
    {
        isInert = false;
        SetPowering();
    }
}

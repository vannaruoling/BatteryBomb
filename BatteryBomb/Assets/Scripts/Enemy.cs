
using UnityEngine;
using System.Collections;
public class Enemy : MonoBehaviour
{
    public int maxHealth = 3;
    public float speed = 2f;
    public bool movementEnabled = true;
    public Transform[] waypoints;

    public GameObject chainExplosionEffect;
    public float chainExplosionRadius = 1.5f;
    public float chainDetonateDelay = 1f;
    public float chainShakeMagnitude = 0.05f;

    public Color chainWarningColor = Color.cyan;
    public float chainWarningFlickerRate = 0.08f;
    public float chainBloatScale = 1.4f;
    public float spriteForwardOffset = -90;

    public float chainShakeDuration = 0.15f;
    public float chainShakeEffectMagnitude = 0.12f;

    public float speedGrowthPerWave = 0.05f;
    public float maxSpeed = 4f;
    private Vector3 baseScale;
    private bool hasCascaded = false;

    private int waypointIndex = 0;
    private Animator animator;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer flashOverlay;
    public Material flashMaterial;


    // Used to avoid null reference from race condition
    void Awake()
    {
        currentHealth = maxHealth;
        if (RoundManager.Instance != null)
        {
            speed = Mathf.Min(maxSpeed, speed + (RoundManager.Instance.WavesPlayed * speedGrowthPerWave));
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        baseScale = transform.localScale;

        GameObject overlayObj = new GameObject("FlashOverlay");
        overlayObj.transform.SetParent(transform);
        overlayObj.transform.localPosition = Vector3.zero;
        overlayObj.transform.localScale = Vector3.one;
        flashOverlay = overlayObj.AddComponent<SpriteRenderer>();
        flashOverlay.material = flashMaterial; // new
        flashOverlay.enabled = false;
        flashOverlay.sortingLayerID = spriteRenderer.sortingLayerID;
        flashOverlay.sortingOrder = spriteRenderer.sortingOrder + 1;
    }

    // void Awake()
    // {
    //     currentHealth = maxHealth;

    //     // Add here to stow it away frontloaded
    //     spriteRenderer = GetComponent<SpriteRenderer>();
    //     animator = GetComponent<Animator>();
    // }

    void Update()
    {
        if (movementEnabled)
        {
            if (waypoints != null && waypointIndex < waypoints.Length)
            {
                Transform target = waypoints[waypointIndex];
                if (target == null)
                {
                    waypointIndex++;
                }
                else
                {
                    Vector3 dir = (target.position - transform.position).normalized;
                    if (dir.sqrMagnitude > 0.0001f)
                    {
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        transform.rotation = Quaternion.Euler(0f, 0f, angle + spriteForwardOffset);
                    }

                    transform.position = Vector3.MoveTowards(
                        transform.position, target.position, speed * Time.deltaTime);

                    if (Vector3.Distance(transform.position, target.position) < 0.05f)
                        waypointIndex++;
                }
            }
            else
            {
                // If no path exists, just move right i guess...
                // TODO: test
                transform.Translate(Vector2.right * speed * Time.deltaTime, Space.World);
            }
        }

        if (flashOverlay != null)
        {
            flashOverlay.sprite = spriteRenderer.sprite;
            // Debug.Log("Copying sprite: " + spriteRenderer.sprite);
        }

    }

    // void Update()
    // {
    //     if (movementEnabled)
    //     {
    //         transform.Translate(Vector2.right * speed * Time.deltaTime, Space.World);
    //     }
    //     if (flashOverlay != null)
    //     {
    //         flashOverlay.sprite = spriteRenderer.sprite;
    //     }
    // }

    System.Collections.IEnumerator FlashRoutine()
    {
        flashOverlay.enabled = true;
        yield return new WaitForSeconds(0.05f);
        if (flashOverlay != null) flashOverlay.enabled = false;
    }



    public void SetPath(Transform[] path)
    {
        waypoints = path;
        waypointIndex = 0;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Enemy hurt: " + amount + ", curr health: " + currentHealth);

        AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyHit, 0.2f);


        if (flashOverlay != null)
        {
            StopCoroutine(nameof(FlashRoutine));
            StartCoroutine(FlashRoutine());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        RoundManager.Instance.ReportEnemyDeath();
        movementEnabled = false;

        if (animator != null)
        {
            animator.Play("dead");
        }

        AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyDeath, 0.6f);

        Color deathColor = new Color(1f, 0f, 0f, 0f); // red, translucent
        Juice.Instance.FadeSpriteToColor(spriteRenderer, deathColor, 0.1f, () =>
            {
                if (this != null) Destroy(gameObject);
            });
    }

    // Chain damage occurs if they get hit by one
    public void TakeChainDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Enemy hurt (chain): " + amount + ", curr health: " + currentHealth);

        if (flashOverlay != null)
        {
            StopCoroutine(nameof(FlashRoutine));
            StartCoroutine(FlashRoutine());
        }

        if (currentHealth <= 0 && !hasCascaded)
        {
            hasCascaded = true;
            StartCoroutine(ChainDeathRoutine(amount));
        }
    }


    // Animation effect for when enemy suffers chain death
    IEnumerator ChainDeathRoutine(int inheritedDamage)
    {
        movementEnabled = false;

        Coroutine flicker = StartCoroutine(ChainWarningFlicker());
        StartCoroutine(ChainBloatRoutine());

        yield return new WaitForSeconds(chainDetonateDelay);

        if (flicker != null) StopCoroutine(flicker);
        if (flashOverlay != null) flashOverlay.enabled = false;

        if (this == null) yield break;

        TriggerChainExplosion(inheritedDamage);
        Die();
    }

    IEnumerator ChainWarningFlicker()
    {
        while (true)
        {
            if (flashOverlay != null)
            {
                flashOverlay.color = chainWarningColor;
                flashOverlay.enabled = !flashOverlay.enabled;
            }
            yield return new WaitForSeconds(chainWarningFlickerRate);
        }
    }

    IEnumerator ChainBloatRoutine()
    {
        float t = 0f;
        while (t < chainDetonateDelay)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / chainDetonateDelay);
            transform.localScale = Vector3.Lerp(baseScale, baseScale * chainBloatScale, p);
            yield return null;
        }
    }


    // Only triggered if the enemy dies to the chain explosion
    void TriggerChainExplosion(int damage)
    {
        if (chainExplosionEffect != null)
        {
            GameObject explosion = Instantiate(chainExplosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 1f);
        }

        Juice.Instance.ShakeTransform(transform, chainShakeEffectMagnitude, chainDetonateDelay);
        AudioManager.Instance.PlaySFX(AudioManager.Instance.chainExplode, 0.35f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, chainExplosionRadius, LayerMask.GetMask("Default"));
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeChainDamage(damage);
            }
        }
    }

    // void Die()
    // {
    //     RoundManager.Instance.ReportEnemyDeath();
    //     Debug.Log("Enemy died");
    //     Destroy(gameObject);
    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }
}


using UnityEngine;
public class Enemy : MonoBehaviour
{
    public int maxHealth = 3;
    public float speed = 2f;
    public bool movementEnabled = true;
    private Animator animator;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer flashOverlay;
    public Material flashMaterial;


    // Used to avoid null reference from race condition
    void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

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
            transform.Translate(Vector2.right * speed * Time.deltaTime, Space.World);
        }
        if (flashOverlay != null)
        {
            flashOverlay.sprite = spriteRenderer.sprite;
        }
    }

    System.Collections.IEnumerator FlashRoutine()
    {
        flashOverlay.enabled = true;
        yield return new WaitForSeconds(0.05f);
        if (flashOverlay != null) flashOverlay.enabled = false;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Enemy hurt: " + amount + ", curr health: " + currentHealth);

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

        Color deathColor = new Color(1f, 0f, 0f, 0f); // red, translucent
        Juice.Instance.FadeSpriteToColor(spriteRenderer, deathColor, 0.1f, () =>
            {
                if (this != null) Destroy(gameObject);
            });
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

using UnityEngine;

public class TurretPlacer : MonoBehaviour
{
    public GameObject turretPrefab;
    public GameObject ghostPrefab;
    // Adjust for snapping
    public float placementCheckRadius = 0.3f;
    public Color validColor = new Color(0.4f, 1f, 0.4f, 0.6f);
    public Color invalidColor = new Color(1f, 0.4f, 0.4f, 0.6f);

    public GameObject rangeIndicatorPrefab;
    private SpriteRenderer rangeIndicatorRenderer;
    private float rangeIndicatorBaseAlpha = 0.5f;
    private GameObject rangeIndicatorInstance;

    private GameObject ghostInstance;
    private SpriteRenderer ghostRenderer;
    private Camera mainCamera;
    private System.Action onPlaced;
    private bool active = false;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    public void BeginPlacement(System.Action onPlacedCallback)
    {
        onPlaced = onPlacedCallback;
        active = true;

        ghostInstance = Instantiate(ghostPrefab);
        ghostRenderer = ghostInstance.GetComponent<SpriteRenderer>();
        ghostRenderer.sortingOrder = 30;


        if (rangeIndicatorPrefab != null)
        {
            rangeIndicatorInstance = Instantiate(rangeIndicatorPrefab, ghostInstance.transform);
            rangeIndicatorInstance.transform.localPosition = Vector3.zero;
            rangeIndicatorRenderer = rangeIndicatorInstance.GetComponent<SpriteRenderer>();
            rangeIndicatorBaseAlpha = rangeIndicatorRenderer.color.a;

            float turretRange = turretPrefab.GetComponent<TurretBase>().range;
            float nativeDiameter = rangeIndicatorRenderer.sprite.bounds.size.x;
            float desiredDiameter = turretRange * 2f;
            rangeIndicatorInstance.transform.localScale = Vector3.one * (desiredDiameter / nativeDiameter);
        }
    }

    void Update()
    {
        if (!active) return;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        ghostInstance.transform.position = mouseWorldPos;

        bool valid = IsValidPlacement(mouseWorldPos);
        ghostRenderer.color = valid ? validColor : invalidColor;

        // Make the range indicator apepar and colour change too
        if (rangeIndicatorRenderer != null)
        {
            Color c = valid ? Color.white : invalidColor;
            c.a = rangeIndicatorBaseAlpha;
            rangeIndicatorRenderer.color = c;
        }


        if (Input.GetMouseButtonDown(0) && valid)
        {
            Commit(mouseWorldPos);
        }
    }


    bool IsValidPlacement(Vector3 pos)
    {
        if (Physics2D.OverlapCircle(pos, placementCheckRadius, LayerMask.GetMask("Path")) != null)
            return false;

        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, placementCheckRadius, LayerMask.GetMask("Default"));
        foreach (Collider2D hit in hits)
        {
            if (hit.GetComponent<TurretBase>() != null) return false;
        }

        return true;
    }

    void Commit(Vector3 pos)
    {
        Instantiate(turretPrefab, pos, Quaternion.identity);
        Destroy(ghostInstance);
        active = false;

        System.Action callback = onPlaced;
        onPlaced = null;
        callback?.Invoke();
    }
}
using UnityEngine;
using TMPro;

public class BatteryBomb : MonoBehaviour
{

    public float countdownTime = 10f;
    public TextMeshProUGUI countdownText;
    public float attachRadius = 1f;

    private bool isDragging = false;
    private Camera mainCamera;
    private Turret attachedTurret = null;


    void Awake()
    {
        mainCamera = Camera.main;
        SetPowering();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCountdownDisplay();

        if (isDragging)
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            transform.position = mouseWorldPos;
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
        if (attachedTurret != null)
        {
            Detach();
        }

        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;

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
            Turret turret = hit.GetComponent<Turret>();
            if (turret != null)
            {
                Debug.Log("Found turret to attach to: " + hit.gameObject.name);
                attachedTurret = turret;
                transform.position = turret.transform.position + new Vector3(0f, 0.5f, 0f);
                attachedTurret.SetPowered(true);
                SetPowering();
                return; // stop after attaching to the first valid turret found
            }
        }

        Debug.Log("No turret found to attach to");

    }

    void Detonate()
    {
        // TODO: detonate properlhy
        Debug.Log("Battery BOOOOOMMMBBB");
        attachedTurret.SetPowered(false);
        Destroy(gameObject);
    }
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {

    // }


}

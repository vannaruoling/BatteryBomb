using UnityEngine;
using TMPro;

public class BatteryBomb : MonoBehaviour
{

    public float countdownTime = 10f;
    public TextMeshProUGUI countdownText;

    private bool isDragging = false;
    private Camera mainCamera;


    void Awake()
    {
        mainCamera = Camera.main;
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
    }

    void UpdateCountdownDisplay()
    {
        if (countdownText != null)
        {
            // round up so that 0 is gonezo
            countdownText.text = Mathf.CeilToInt(countdownTime).ToString();
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;
        // Attachment check will go here in the next step
    }
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {

    // }


}

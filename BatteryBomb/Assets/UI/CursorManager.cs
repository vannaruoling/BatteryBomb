using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [Header("Cursor Textures")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D clickCursor;

    [Header("Cursor Settings")]
    [SerializeField] private Vector2 hotSpot = Vector2.zero;

    void Start()
    {
        SetCustomCursor(defaultCursor);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetCustomCursor(clickCursor);
        }

        if (Input.GetMouseButtonUp(0))
        {
            SetCustomCursor(defaultCursor);
        }
    }

    private void SetCustomCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }
}
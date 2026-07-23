using UnityEngine;
using UnityEngine.InputSystem; // Added New Input System namespace

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
        // Make sure a mouse device is present
        if (Mouse.current == null) return;

        // Check if left button was pressed down this frame
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            SetCustomCursor(clickCursor);
        }

        // Check if left button was released this frame
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            SetCustomCursor(defaultCursor);
        }
    }

    private void SetCustomCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }
}
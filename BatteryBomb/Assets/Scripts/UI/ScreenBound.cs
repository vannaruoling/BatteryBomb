using UnityEngine;

public static class ScreenBounds
{
    public static Vector3 ClampToCameraView(Vector3 worldPos, Camera cam, float paddingWorldUnits)
    {
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float minX = cam.transform.position.x - halfWidth + paddingWorldUnits;
        float maxX = cam.transform.position.x + halfWidth - paddingWorldUnits;
        float minY = cam.transform.position.y - halfHeight + paddingWorldUnits;
        float maxY = cam.transform.position.y + halfHeight - paddingWorldUnits;

        worldPos.x = Mathf.Clamp(worldPos.x, minX, maxX);
        worldPos.y = Mathf.Clamp(worldPos.y, minY, maxY);
        return worldPos;
    }
}
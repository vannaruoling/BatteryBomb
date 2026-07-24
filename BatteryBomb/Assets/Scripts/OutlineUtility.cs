using UnityEngine;

// Reusable outline component to add a white outline effect on any object
// assumes a 1.1x scaling

public static class OutlineUtility
{
    public static SpriteRenderer CreateOutline(Transform parent, SpriteRenderer sourceRenderer, float scale = 1.1f)
    {
        GameObject outlineObj = new GameObject("Outline");
        outlineObj.transform.SetParent(parent);
        outlineObj.transform.localPosition = Vector3.zero;
        outlineObj.transform.localScale = Vector3.one * scale;

        SpriteRenderer outlineRenderer = outlineObj.AddComponent<SpriteRenderer>();
        outlineRenderer.sprite = sourceRenderer.sprite;
        outlineRenderer.color = Color.white;
        outlineRenderer.sortingOrder = 5;

        outlineObj.SetActive(false);
        return outlineRenderer;
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// "Game feel" juice, purely visual, should be pausable
public class Juice : MonoBehaviour
{
    public static Juice Instance;

    private Dictionary<SpriteRenderer, Coroutine> activeFlashes = new Dictionary<SpriteRenderer, Coroutine>();
    private Dictionary<SpriteRenderer, Color> flashOriginals = new Dictionary<SpriteRenderer, Color>();

    private Dictionary<Transform, Coroutine> activeShakes = new Dictionary<Transform, Coroutine>();
    private Dictionary<Transform, Vector3> shakeOriginals = new Dictionary<Transform, Vector3>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FlashSprite(SpriteRenderer sr, Color flashColor, float duration)
    {
        if (sr == null) return;

        // if already flashing, stop and restore before capturing a new original or else stays white...
        if (activeFlashes.TryGetValue(sr, out Coroutine running))
        {
            if (running != null) StopCoroutine(running);
            sr.color = flashOriginals[sr];
            activeFlashes.Remove(sr);
            flashOriginals.Remove(sr);
        }

        flashOriginals[sr] = sr.color;
        activeFlashes[sr] = StartCoroutine(FlashRoutine(sr, flashColor, duration));
    }

    IEnumerator FlashRoutine(SpriteRenderer sr, Color flashColor, float duration)
    {
        sr.color = flashColor;
        yield return new WaitForSeconds(duration);

        // sr may have been destroyed mid-flash e.g. enemy dies
        if (sr != null && flashOriginals.ContainsKey(sr))
        {
            sr.color = flashOriginals[sr];
        }

        activeFlashes.Remove(sr);
        flashOriginals.Remove(sr);
    }

    public void ShakeTransform(Transform t, float magnitude, float duration)
    {
        if (t == null) return;

        if (activeShakes.TryGetValue(t, out Coroutine running))
        {
            if (running != null) StopCoroutine(running);
            t.localPosition = shakeOriginals[t];
            activeShakes.Remove(t);
            shakeOriginals.Remove(t);
        }

        shakeOriginals[t] = t.localPosition;
        activeShakes[t] = StartCoroutine(ShakeRoutine(t, magnitude, duration));
    }

    IEnumerator ShakeRoutine(Transform t, float magnitude, float duration)
    {
        Vector3 original = shakeOriginals[t];
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (t == null) yield break;

            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            t.localPosition = original + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (t != null)
        {
            t.localPosition = original;
        }

        activeShakes.Remove(t);
        shakeOriginals.Remove(t);
    }


    public void FadeSpriteToColor(SpriteRenderer sr, Color targetColor, float duration, System.Action onComplete = null)
    {
        if (sr == null) return;
        StartCoroutine(FadeRoutine(sr, targetColor, duration, onComplete));
    }

    IEnumerator FadeRoutine(SpriteRenderer sr, Color targetColor, float duration, System.Action onComplete)
    {
        Color start = sr.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (sr == null) yield break;
            sr.color = Color.Lerp(start, targetColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        onComplete?.Invoke();
    }

}
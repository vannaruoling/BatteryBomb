using UnityEngine;
using TMPro;
using System.Collections;

public class DamageFlashDisplay : MonoBehaviour
{
    public static DamageFlashDisplay Instance;

    public TextMeshProUGUI flashText;
    public float displayDuration = 0.4f;
    public float startScale = 1.5f;
    public float endScale = 1f;
    public float restingScale = 1f;
    public float restingAlpha = 1f;


    private Coroutine activeFlash;

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

    public void ShowDamage(int amount)
    {
        if (activeFlash != null)
        {
            StopCoroutine(activeFlash);
        }
        activeFlash = StartCoroutine(FlashRoutine(amount));
    }


    IEnumerator FlashRoutine(int healthRemaining)
    {
        flashText.text = healthRemaining.ToString();
        flashText.transform.localScale = Vector3.one * startScale;
        flashText.alpha = 1f;

        float t = 0f;
        while (t < displayDuration)
        {
            t += Time.deltaTime;
            float progress = t / displayDuration;

            flashText.transform.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one * restingScale, progress);
            flashText.alpha = Mathf.Lerp(1f, restingAlpha, progress);

            yield return null;
        }

        flashText.transform.localScale = Vector3.one * restingScale;
        flashText.alpha = restingAlpha;
    }
}
using System.Collections;
using TMPro;
using UnityEngine;

public class CounterDisplay : MonoBehaviour
{
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI subText;
    public string subPrefix = "Next: ";

    public float rollDuration = 0.45f;
    public int minRollDistance = 8;
    public float enterOffset = 26f;
    public float enterAlpha = 0.15f;
    public float tickSettle = 0.12f;
    public float landPunchScale = 1.15f;
    public float landPunchDuration = 0.12f;
    public bool useUnscaledTime = true;

    public int MainValue { get; private set; }
    public int SubValue { get; private set; }

    private Coroutine mainRoutine;
    private Coroutine subRoutine;
    private Vector3 mainHomePos, mainHomeScale;
    private Vector3 subHomePos, subHomeScale;
    private bool cached;
    private bool warned;

    void Awake()
    {
        CacheHomes();
    }

    void CacheHomes()
    {
        if (cached) return;

        if (mainText != null)
        {
            mainHomePos = mainText.transform.localPosition;
            mainHomeScale = mainText.transform.localScale;
        }
        if (subText != null)
        {
            subHomePos = subText.transform.localPosition;
            subHomeScale = subText.transform.localScale;
        }
        cached = true;
    }

    public void SetValue(int value, bool instant = false)
    {
        CacheHomes();
        if (mainText == null) { WarnMissing("mainText"); return; }

        int from = MainValue;
        MainValue = value;

        StopRoutine(ref mainRoutine);

        if (instant || from == value || !isActiveAndEnabled)
        {
            Snap(mainText, "", value, mainHomePos, mainHomeScale);
            return;
        }

        mainRoutine = StartCoroutine(Roll(
            mainText, "", from, value, mainHomePos, mainHomeScale,
            () => mainRoutine = null));
    }

    public void SetSubValue(int value, bool instant = false)
    {
        CacheHomes();
        if (subText == null) { WarnMissing("subText"); return; }

        int from = SubValue;
        SubValue = value;

        StopRoutine(ref subRoutine);

        if (instant || from == value || !isActiveAndEnabled)
        {
            Snap(subText, subPrefix, value, subHomePos, subHomeScale);
            return;
        }

        subRoutine = StartCoroutine(Roll(
            subText, subPrefix, from, value, subHomePos, subHomeScale,
            () => subRoutine = null));
    }

    public void SetSubPrefix(string prefix)
    {
        subPrefix = prefix;
        if (subText != null && subRoutine == null)
        {
            CacheHomes();
            Snap(subText, subPrefix, SubValue, subHomePos, subHomeScale);
        }
    }

    public void SetSubVisible(bool visible)
    {
        if (subText != null) subText.gameObject.SetActive(visible);
    }

    public void Set(int main, int sub, bool instant = false)
    {
        SetValue(main, instant);
        SetSubValue(sub, instant);
    }

    IEnumerator Roll(TextMeshProUGUI label, string prefix, int from, int to,
                     Vector3 homePos, Vector3 homeScale, System.Action onDone)
    {
        float enterSign = to < from ? 1f : -1f;


        // If its just one number change, dont do the whole spinning animation
        bool isSingleStep = Mathf.Abs(to - from) <= 1;

        if (!isSingleStep)
        {
            // Guarantee a visible spin even when the value only moved by more than 1.
            int start = from;
            if (Mathf.Abs(to - from) < minRollDistance)
            {
                start = to + (to < from ? minRollDistance : -minRollDistance);
            }

            float t = 0f;
            float tick = 0f;
            int lastShown = int.MinValue;

            while (t < rollDuration)
            {
                float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                t += dt;
                tick += dt;

                float p = Mathf.Clamp01(t / rollDuration);
                float eased = 1f - Mathf.Pow(1f - p, 3f);
                int shown = Mathf.RoundToInt(Mathf.Lerp(start, to, eased));

                if (shown != lastShown)
                {
                    lastShown = shown;
                    label.text = prefix + shown;
                    tick = 0f;
                }

                float tp = tickSettle <= 0f ? 1f : Mathf.Clamp01(tick / tickSettle);
                label.transform.localPosition = homePos + Vector3.up * (enterOffset * enterSign * (1f - tp));
                label.alpha = Mathf.Lerp(enterAlpha, 1f, tp);

                yield return null;
                if (label == null) yield break;
            }
        }
        else
        {

            label.text = prefix + to;

            float t = 0f;
            while (t < tickSettle)
            {
                float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                t += dt;
                float tp = Mathf.Clamp01(t / tickSettle);

                label.transform.localPosition = homePos + Vector3.up * (enterOffset * enterSign * (1f - tp));
                label.alpha = Mathf.Lerp(enterAlpha, 1f, tp);

                yield return null;
                if (label == null) yield break;
            }
        }

        // int start = from;
        // if (Mathf.Abs(to - from) < minRollDistance)
        // {
        //     start = to + (to < from ? minRollDistance : -minRollDistance);
        // }

        // float t = 0f;
        // float tick = 0f;
        // int lastShown = int.MinValue;

        // while (t < rollDuration)
        // {
        //     float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        //     t += dt;
        //     tick += dt;

        //     float p = Mathf.Clamp01(t / rollDuration);
        //     float eased = 1f - Mathf.Pow(1f - p, 3f);
        //     int shown = Mathf.RoundToInt(Mathf.Lerp(start, to, eased));

        //     if (shown != lastShown)
        //     {
        //         lastShown = shown;
        //         label.text = prefix + shown;
        //         tick = 0f;
        //     }

        //     float tp = tickSettle <= 0f ? 1f : Mathf.Clamp01(tick / tickSettle);
        //     label.transform.localPosition = homePos + Vector3.up * (enterOffset * enterSign * (1f - tp));
        //     label.alpha = Mathf.Lerp(enterAlpha, 1f, tp);

        //     yield return null;
        //     if (label == null) yield break;
        // }

        label.text = prefix + to;
        label.transform.localPosition = homePos;
        label.alpha = 1f;


        // Snap into place at end
        // TODO: fic that weird lateral movement
        float pt = 0f;
        while (pt < landPunchDuration)
        {
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            pt += dt;
            float pp = Mathf.Clamp01(pt / landPunchDuration);
            label.transform.localScale = homeScale * Mathf.Lerp(landPunchScale, 1f, pp);

            yield return null;
            if (label == null) yield break;
        }

        label.transform.localScale = homeScale;
        onDone?.Invoke();
    }

    void Snap(TextMeshProUGUI label, string prefix, int value, Vector3 homePos, Vector3 homeScale)
    {
        label.text = prefix + value;
        label.transform.localPosition = homePos;
        label.transform.localScale = homeScale;
        label.alpha = 1f;
    }

    void StopRoutine(ref Coroutine routine)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }

    void OnDisable()
    {
        if (!cached) return;

        StopRoutine(ref mainRoutine);
        StopRoutine(ref subRoutine);

        if (mainText != null) Snap(mainText, "", MainValue, mainHomePos, mainHomeScale);
        if (subText != null) Snap(subText, subPrefix, SubValue, subHomePos, subHomeScale);
    }

    void WarnMissing(string field)
    {
        if (warned) return;
        warned = true;
        Debug.LogWarning("CounterDisplay on '" + name + "': field '" + field +
                         "' is not assigned in the Inspector.", this);
    }
}
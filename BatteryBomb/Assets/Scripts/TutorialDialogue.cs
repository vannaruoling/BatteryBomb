using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialDialogue : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI text;

    public IEnumerator Show(string message)
    {
        panel.SetActive(true);
        text.text = message;

        yield return null; // don't eat the click that got us here

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        panel.SetActive(false);
    }
    public IEnumerator ShowAuto(string message, float duration)
    {
        panel.SetActive(true);
        text.text = message;

        yield return new WaitForSeconds(duration);

        panel.SetActive(false);
    }
}
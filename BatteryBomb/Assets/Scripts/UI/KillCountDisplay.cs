using UnityEngine;
using TMPro;

public class KillCountDisplay : MonoBehaviour
{
    public TextMeshProUGUI killCountText;

    void Update()
    {
        killCountText.text = "Kills: " + GameManager.Instance.killCount;
    }
}
using UnityEngine;
using TMPro;

public class HealthDisplay : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    // Update is called once per frame
    void Update()
    {
        healthText.text = "Health: " + GameManager.Instance.playerHealth;
    }
}

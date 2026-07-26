using UnityEngine;

public class UpgradeState : MonoBehaviour
{
    public static UpgradeState Instance;

    public float bombTimerBonus = 0f;
    public float explosionRadiusBonus = 0f;
    public float turretFireRateMultiplier = 1f;
    public int maxBombCountBonus = 0;

    public bool chainExplosionDisabled = false;
    public int explosionDamageBonus = 0;

    public bool chainUnlocked = false;

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
}
using UnityEngine;
using System.Collections.Generic;
public class UpgradeState : MonoBehaviour
{
    [System.Serializable]
    public class TurretUpgrade
    {
        public float fireRateMultiplier = 1f;
        public float rangeBonus = 0f;
    }

    private Dictionary<TurretType, TurretUpgrade> turretUpgrades = new Dictionary<TurretType, TurretUpgrade>();
    public HashSet<TurretType> ownedTypes = new HashSet<TurretType>();

    public TurretUpgrade GetUpgrade(TurretType type)
    {
        if (!turretUpgrades.ContainsKey(type))
            turretUpgrades[type] = new TurretUpgrade();
        return turretUpgrades[type];
    }

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
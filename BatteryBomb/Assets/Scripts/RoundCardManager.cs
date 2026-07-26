using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RoundCardManager : MonoBehaviour
{

    // Option setup for picking a turret
    [System.Serializable]
    public class TurretOption
    {
        public string label;
        public GameObject prefab;
        public Sprite icon;
    }

    public TurretOption[] turretOptions;
    public static RoundCardManager Instance;
    public GameObject[] cards;

    // Card option with a label and effect
    private struct CardOption
    {
        public string label;
        public Sprite icon;
        public System.Action effect;
        public TurretType? requiredType;

        public CardOption(string label, Sprite icon, System.Action effect, TurretType? requiredType = null)
        {
            this.label = label;
            this.icon = icon;
            this.effect = effect;
            this.requiredType = requiredType;
        }
    }

    public Sprite healIcon;
    public Sprite bombTimerIcon;
    public Sprite fireRateIcon;
    public Sprite explosionRadiusIcon;
    public Sprite bombAmmoIcon;
    public Sprite chainUnlockIcon;
    public Sprite rangeIcon;

    public Image[] cardIcons;

    private List<CardOption> allCards;
    private bool selectionMade = false;

    public bool chainUnlocked = false;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }

        allCards = new List<CardOption>
        {
            new CardOption("Heal +2", healIcon, OnCardHeal),
            new CardOption("Bomb Timer +0.5s", bombTimerIcon, OnCardBombTimer),
            new CardOption("Bomb Ammo +1", bombAmmoIcon, OnCardMaxBombCount),
            new CardOption("Unstable Core: explosions chain", chainUnlockIcon, OnCardUnlockChain),

            new CardOption("Basic Turret: Fire Rate +5%", fireRateIcon, () => ApplyFireRate(TurretType.Basic), TurretType.Basic),
            new CardOption("Basic Turret: Range +1", rangeIcon, () => ApplyRange(TurretType.Basic, 0.25f), TurretType.Basic),
            new CardOption("Spread Turret: Fire Rate +5%", fireRateIcon, () => ApplyFireRate(TurretType.Spread), TurretType.Spread),
            new CardOption("Spread Turret: Range +1", rangeIcon, () => ApplyRange(TurretType.Spread, 0.25f), TurretType.Spread),
            new CardOption("Cannon Turret: Fire Rate +5%", fireRateIcon, () => ApplyFireRate(TurretType.Cannon), TurretType.Cannon),
            new CardOption("Cannon Turret: Range +1", rangeIcon, () => ApplyRange(TurretType.Cannon, 0.25f), TurretType.Cannon),
        };
    }

    public void PresentRandomCards()
    {
        selectionMade = false;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.cardsShown, 1.5f);

        List<CardOption> pool = allCards.FindAll(c =>
      (c.requiredType == null || UpgradeState.Instance.ownedTypes.Contains(c.requiredType.Value))
      && (!c.label.StartsWith("Unstable Core") || RoundManager.Instance.WavesPlayed >= 5));

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].SetActive(true);

            int index = Random.Range(0, pool.Count);
            CardOption chosen = pool[index];
            pool.RemoveAt(index);

            Button btn = cards[i].GetComponent<Button>();
            btn.onClick.RemoveAllListeners();

            System.Action effect = chosen.effect;
            btn.onClick.AddListener(() => effect());

            TextMeshProUGUI label = cards[i].GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = chosen.label;
            }
            if (cardIcons != null && i < cardIcons.Length && cardIcons[i] != null)
            {
                cardIcons[i].sprite = chosen.icon;
                cardIcons[i].enabled = chosen.icon != null;
            }
        }
    }

    void OnCardHeal()
    {
        if (selectionMade) return;
        selectionMade = true;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.cardPick);

        GameManager.Instance.HealPlayer(2);
        RoundManager.Instance.StartRound();
    }

    void OnCardBombTimer()
    {
        if (selectionMade) return;
        selectionMade = true;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.cardPick);

        UpgradeState.Instance.bombTimerBonus += 0.5f;
        RoundManager.Instance.StartRound();
    }

    void ApplyFireRate(TurretType type)
    {
        if (selectionMade) return;
        selectionMade = true;

        var upg = UpgradeState.Instance.GetUpgrade(type);
        upg.fireRateMultiplier *= 1.05f;

        foreach (TurretBase t in FindObjectsByType<TurretBase>(FindObjectsSortMode.None))
            if (t.turretType == type) t.fireRate *= 1.05f;

        RoundManager.Instance.StartRound();
    }

    void ApplyRange(TurretType type, float amount)
    {
        if (selectionMade) return;
        selectionMade = true;

        var upg = UpgradeState.Instance.GetUpgrade(type);
        upg.rangeBonus += amount;

        foreach (TurretBase t in FindObjectsByType<TurretBase>(FindObjectsSortMode.None))
            if (t.turretType == type) t.range += amount;

        RoundManager.Instance.StartRound();
    }

    void OnCardUnlockChain()
    {
        if (selectionMade) return;
        selectionMade = true;

        UpgradeState.Instance.chainUnlocked = true;
        allCards.RemoveAll(c => c.label.StartsWith("Unstable Core"));

        RoundManager.Instance.StartRound();
    }

    void OnCardExplosionRadius()
    {
        if (selectionMade) return;
        selectionMade = true;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.cardPick);

        UpgradeState.Instance.explosionRadiusBonus += 0.5f;
        RoundManager.Instance.StartRound();
    }

    void OnCardMaxBombCount()
    {
        if (selectionMade) return;
        selectionMade = true;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.cardPick);

        UpgradeState.Instance.maxBombCountBonus += 1;
        RoundManager.Instance.StartRound();
    }

    public void PresentTurretCards()
    {
        selectionMade = false;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.cardsShown, 1.5f);

        List<TurretOption> pool = new List<TurretOption>(turretOptions);

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].SetActive(true);

            int index = Random.Range(0, pool.Count);
            TurretOption chosen = pool[index];
            pool.RemoveAt(index);

            Button btn = cards[i].GetComponent<Button>();
            btn.onClick.RemoveAllListeners();

            GameObject prefab = chosen.prefab;
            btn.onClick.AddListener(() => OnTurretCardChosen(prefab));

            TextMeshProUGUI label = cards[i].GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = chosen.label;
            }

            if (cardIcons != null && i < cardIcons.Length && cardIcons[i] != null)
            {
                cardIcons[i].sprite = chosen.icon;
                cardIcons[i].enabled = chosen.icon != null;
            }
        }
    }

    void OnTurretCardChosen(GameObject turretPrefab)
    {
        if (selectionMade) return;
        selectionMade = true;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.cardPick);

        RoundManager.Instance.roundCardPanel.SetActive(false);
        RoundManager.Instance.turretPlacer.turretPrefab = turretPrefab;
        RoundManager.Instance.turretPlacer.BeginPlacement(RoundManager.Instance.StartRound);
    }
}
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

        public CardOption(string label, Sprite icon, System.Action effect)
        {
            this.label = label;
            this.icon = icon;
            this.effect = effect;
        }
    }

    public Sprite healIcon;
    public Sprite bombTimerIcon;
    public Sprite fireRateIcon;
    public Sprite explosionRadiusIcon;
    public Sprite bombAmmoIcon;

    public Image[] cardIcons;

    private List<CardOption> allCards;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }

        allCards = new List<CardOption>
        {
            new CardOption("Heal +2", healIcon, OnCardHeal),
            new CardOption("Bomb Timer +3s", bombTimerIcon, OnCardBombTimer),
            new CardOption("Turret Fire Rate +25%", fireRateIcon, OnCardTurretFireRate),
            new CardOption("Explosion Radius +0.5", explosionRadiusIcon, OnCardExplosionRadius),
            new CardOption("Bomb Ammo +1", bombAmmoIcon, OnCardMaxBombCount),
        };
    }

    public void PresentRandomCards()
    {
        List<CardOption> pool = new List<CardOption>(allCards);

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
        AudioManager.Instance.PlaySFX(AudioManager.Instance.cardPick);

        GameManager.Instance.HealPlayer(2);
        RoundManager.Instance.StartRound();
    }

    void OnCardBombTimer()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.cardPick);

        UpgradeState.Instance.bombTimerBonus += 3f;
        RoundManager.Instance.StartRound();
    }

    void OnCardTurretFireRate()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.cardPick);

        UpgradeState.Instance.turretFireRateMultiplier *= 1.25f;

        TurretBase[] turrets = FindObjectsByType<TurretBase>(FindObjectsSortMode.None);
        foreach (TurretBase t in turrets)
        {
            t.fireRate *= 1.25f;
        }

        RoundManager.Instance.StartRound();
    }

    void OnCardExplosionRadius()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.cardPick);

        UpgradeState.Instance.explosionRadiusBonus += 0.5f;
        RoundManager.Instance.StartRound();
    }

    void OnCardMaxBombCount()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.cardPick);

        UpgradeState.Instance.maxBombCountBonus += 1;
        RoundManager.Instance.StartRound();
    }

    public void PresentTurretCards()
    {
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
        AudioManager.Instance.PlaySFX(AudioManager.Instance.cardPick);

        RoundManager.Instance.roundCardPanel.SetActive(false);
        RoundManager.Instance.turretPlacer.turretPrefab = turretPrefab;
        RoundManager.Instance.turretPlacer.BeginPlacement(RoundManager.Instance.StartRound);
    }

}
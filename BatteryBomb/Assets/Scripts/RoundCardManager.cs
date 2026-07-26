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
    }

    public TurretOption[] turretOptions;
    public static RoundCardManager Instance;
    public GameObject[] cards;

    // Card option with a label and effect
    private struct CardOption
    {
        public string label;
        public System.Action effect;

        public CardOption(string label, System.Action effect)
        {
            this.label = label;
            this.effect = effect;
        }
    }

    private List<CardOption> allCards;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }

        allCards = new List<CardOption>
        {
            // TODO: ADd any new buffs here.
            new CardOption("Heal +2", OnCardHeal),
            new CardOption("Bomb Timer +3s", OnCardBombTimer),
            new CardOption("Turret Fire Rate +25%", OnCardTurretFireRate),
            new CardOption("Explosion Radius +0.5", OnCardExplosionRadius),
            new CardOption("Bomb Ammo +1", OnCardMaxBombCount),
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
        }
    }

    void OnCardHeal()
    {
        GameManager.Instance.HealPlayer(2);
        RoundManager.Instance.StartRound();
    }

    void OnCardBombTimer()
    {
        UpgradeState.Instance.bombTimerBonus += 3f;
        RoundManager.Instance.StartRound();
    }

    void OnCardTurretFireRate()
    {
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
        UpgradeState.Instance.explosionRadiusBonus += 0.5f;
        RoundManager.Instance.StartRound();
    }

    void OnCardMaxBombCount()
    {
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
        }
    }

    void OnTurretCardChosen(GameObject turretPrefab)
    {
        RoundManager.Instance.roundCardPanel.SetActive(false);
        RoundManager.Instance.turretPlacer.turretPrefab = turretPrefab;
        RoundManager.Instance.turretPlacer.BeginPlacement(RoundManager.Instance.StartRound);
    }

}
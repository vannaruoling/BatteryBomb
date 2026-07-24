using UnityEngine;

public class RoundCardManager : MonoBehaviour
{
    // Hard coded buff for now; add better ones later
    public int healAmount = 2;

    public void OnCardSelected()
    {
        GameManager.Instance.HealPlayer(healAmount);
        RoundManager.Instance.StartRound();
    }
}

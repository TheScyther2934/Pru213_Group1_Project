using UnityEngine;
using UnityEngine.Events;

public class CurrencyWallet : MonoBehaviour
{
    public int gold = 0;
    public UnityEvent<int> OnGoldChanged;

    public void AddGold(int amount)
    {
        gold += Mathf.Max(0, amount);
        OnGoldChanged?.Invoke(gold);
    }

    public bool SpendGold(int amount)
    {
        if (gold < amount) return false;
        gold -= amount;
        OnGoldChanged?.Invoke(gold);
        return true;
    }
}

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GoldPickup : MonoBehaviour
{
    public int amount = 1;

    void Awake() { var c = GetComponent<Collider2D>(); c.isTrigger = true; }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var wallet = other.GetComponent<CurrencyWallet>();
        if (!wallet) return;
        wallet.AddGold(amount);
        Destroy(gameObject);
    }
}

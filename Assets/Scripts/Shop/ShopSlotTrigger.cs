using UnityEngine;

public class ShopSlotTrigger : MonoBehaviour
{
    public int index;                // 0,1,2
    public ShopVendor vendor;        // kéo ShopVendor vào
    public Transform anchor;         // điểm neo tooltip (kéo transform trên bệ)

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var ps = other.GetComponent<PlayerStats>();
        var w  = other.GetComponent<CurrencyWallet>();
        vendor.OnEnterSlot(index, anchor ? anchor : transform, ps, w);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        vendor.OnExitSlot(index);
    }
}

using UnityEngine;

public class ShopSlotTrigger : MonoBehaviour
{
    public int index;           // 0,1,2
    public ShopVendor vendor;   // kéo ShopVendor vào Inspector

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var ps = other.GetComponent<PlayerStats>();
        var w  = other.GetComponent<CurrencyWallet>();

        vendor.SetPlayer(ps, w);          // 👈 set player cho vendor
        vendor.SetSelectedSlot(index);    // 👈 chọn đúng slot
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player")) vendor.SetSelectedSlot(index);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        vendor.OnLeaveSlot(index, other.GetComponent<PlayerStats>());
    }
}

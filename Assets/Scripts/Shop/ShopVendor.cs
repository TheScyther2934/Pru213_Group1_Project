using UnityEngine;

[System.Serializable]
public class ShopEntry
{
    public ItemSO item;
    public int price = 10;
    public int stock = -1; // -1 = vô hạn
}

public class ShopVendor : MonoBehaviour
{
    public ShopEntry[] goods;  // cấu hình trong Inspector
    public KeyCode buyKey = KeyCode.B;

    bool playerInRange;
    CurrencyWallet wallet;
    Stash stash;
    PlayerStats pstats;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        wallet = other.GetComponent<CurrencyWallet>();
        stash = other.GetComponent<Stash>();
        pstats = other.GetComponent<PlayerStats>();
        Debug.Log("Shop: nhấn B để mua món [0] (demo) — bạn có thể làm UI sau.");
    }

    void OnTriggerExit2D(Collider2D other) { if (other.CompareTag("Player")) playerInRange = false; }

    void Update()
    {
        if (!playerInRange || wallet == null || stash == null) return;
        if (Input.GetKeyDown(buyKey)) TryBuy(0); // demo mua món đầu tiên
    }

    public bool TryBuy(int index)
    {
        if (index < 0 || index >= goods.Length) return false;
        var e = goods[index];
        if (!e.item) return false;

        if (e.stock == 0) { Debug.Log("Hết hàng!"); return false; }
        if (!wallet.SpendGold(e.price)) { Debug.Log("Không đủ vàng!"); return false; }

        stash.Add(e.item, 1);      // 👉 đưa vào RƯƠNG
        if (e.stock > 0) e.stock--;
        Debug.Log($"Đã mua {e.item.displayName} và cất vào rương.");
        return true;
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ShopSlot
{
    [HideInInspector] public PermanentStatSO item;
    [HideInInspector] public bool sold;
}

public class ShopVendor : MonoBehaviour
{
    [Header("Catalog (kéo tất cả asset PermanentStatSO vào đây)")]
    public List<PermanentStatSO> catalog = new List<PermanentStatSO>();

    [Header("3 vị trí hiển thị (mỗi vị trí có component ShopItemDisplay)")]
    public ShopItemDisplay[] displays = new ShopItemDisplay[3];

    [Header("Input")]
    public KeyCode openKey = KeyCode.O;        // mở/đóng shop nếu bạn muốn gắn UI
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode buyKey = KeyCode.Return;

    [Header("State")]
    public ShopSlot[] slots = new ShopSlot[3] { new(), new(), new() };
    int selected = 0;
    bool isOpen = true; // dùng tạm: mở sẵn. Nếu bạn muốn phải nhấn O mới mua, set false rồi toggle bằng O.

    // refs runtime
    CurrencyWallet wallet;
    PlayerStats pstats;

    void Start()
    {
        if (catalog == null || catalog.Count == 0) {
            Debug.LogWarning("ShopVendor: catalog rỗng!");
        }
        RandomizeOffers();
        RefreshDisplays();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) return;
        wallet = other.GetComponent<CurrencyWallet>();
        pstats = other.GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (Input.GetKeyDown(openKey)) { isOpen = !isOpen; UpdateHighlights(); }

        if (!isOpen) return;

        if (Input.GetKeyDown(leftKey)) { selected = Mathf.Max(0, selected - 1); UpdateHighlights(); }
        if (Input.GetKeyDown(rightKey)) { selected = Mathf.Min(displays.Length - 1, selected + 1); UpdateHighlights(); }

        if (Input.GetKeyDown(buyKey)) TryBuy(selected);
    }

    void UpdateHighlights() {
        for (int i = 0; i < displays.Length; i++)
            if (displays[i]) displays[i].SetHighlight(isOpen && i == selected);
    }

    void RandomizeOffers()
    {
        // Lấy tối đa 3 item ngẫu nhiên, không trùng, từ catalog
        var pool = new List<PermanentStatSO>(catalog.Where(x => x != null));
        for (int i = 0; i < 3; i++) {
            slots[i].sold = false;
            if (pool.Count > 0) {
                int idx = Random.Range(0, pool.Count);
                slots[i].item = pool[idx];
                pool.RemoveAt(idx);
            } else {
                slots[i].item = null;
            }
        }
        selected = 0;
    }

    void RefreshDisplays()
    {
        for (int i = 0; i < displays.Length; i++) {
            var d = displays[i];
            if (!d) continue;
            d.Show(slots[i].item);
            d.SetSold(slots[i].sold);
        }
        UpdateHighlights();
    }

    public bool TryBuy(int index)
    {
        if (index < 0 || index >= slots.Length) return false;
        var s = slots[index];
        var item = s.item;
        if (item == null) return false;
        if (s.sold) { Debug.Log("Món này đã bán."); return false; }
        if (wallet == null || pstats == null) { Debug.Log("Chưa có player trong vùng shop."); return false; }
        if (!wallet.SpendGold(item.price)) { Debug.Log("Không đủ vàng!"); return false; }

        // Áp dụng ngay
        item.ApplyTo(pstats);
        s.sold = true;
        slots[index] = s;

        // cập nhật hiển thị
        if (displays[index]) displays[index].SetSold(true);

        // nếu cả 3 đã bán -> reset
        if (slots.All(x => x.item == null || x.sold)) {
            Debug.Log("Hết hàng – reset shop!");
            RandomizeOffers();
            RefreshDisplays();
        }
        return true;
    }

public void SetPlayer(PlayerStats ps, CurrencyWallet w)
{
    pstats = ps;
    wallet = w;
    isOpen = true;          // nếu muốn auto mở khi đứng vào slot
    UpdateHighlights();
}

public void SetSelectedSlot(int index)
{
    selected = Mathf.Clamp(index, 0, displays.Length - 1);
    UpdateHighlights();
}

public void OnLeaveSlot(int idx, PlayerStats ps)
{
    // Nếu muốn khi ra khỏi slot thì bỏ highlight
    if (selected == idx && displays[idx] != null)
        displays[idx].SetHighlight(false);

    // Nếu player rời xa hết các slot, có thể clear player:
    // pstats = null; wallet = null; isOpen = false;
}


}

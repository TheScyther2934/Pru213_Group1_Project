using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ShopSlot
{
    [HideInInspector] public ItemSO item;
    [HideInInspector] public bool sold;
}

public class ShopVendor : MonoBehaviour
{
    [Header("Catalog")]
    public List<ItemSO> catalog = new List<ItemSO>();

    [Header("Displays (kéo 3 ShopItemDisplay của Slot0/1/2)")]
    public ShopItemDisplay[] displays = new ShopItemDisplay[3];

    [Header("Popup UI (kéo từ Canvas)")]
    public GameObject infoPanel;   // popup giữa màn hình (dùng chung)
    public GameObject dimmer;      // tấm đen full-screen (dùng chung, mặc định tắt)

    [Header("Input")]
    public KeyCode infoKey  = KeyCode.Return; // Enter: mở/đóng popup cho slot đang chọn
    public KeyCode closeKey = KeyCode.Escape; // Esc: đóng popup
    public KeyCode buyKey   = KeyCode.B;      // B: mua

    [Header("State")]
    public ShopSlot[] slots = new ShopSlot[3] { new(), new(), new() };
    int selected = 0;
    bool isOpen = true;
    bool isInfoOpen = false; // trạng thái popup

    CurrencyWallet wallet;
    PlayerStats pstats;

    void Start()
    {
        if (catalog == null || catalog.Count == 0)
            Debug.LogWarning("ShopVendor: catalog rỗng!");

        // Ẩn popup + dimmer khi vào game
        SetPopup(false);

        RandomizeOffers();
        RefreshDisplays();
    }

    void Update()
    {
        if (!isOpen) return;

        if (Input.GetKeyDown(infoKey))
            ToggleInfoForSelected();

        if (Input.GetKeyDown(closeKey))
            CloseInfo();

        if (Input.GetKeyDown(buyKey))
        {
            if (TryBuy(selected))
                CloseInfo(); // mua xong ẩn info
        }
    }

    // ================= POPUP =================
    void ToggleInfoForSelected()
    {
        if (selected < 0 || selected >= displays.Length) return;

        // Không cho mở info nếu slot rỗng hoặc đã bán
        if (slots[selected].item == null || slots[selected].sold)
        {
            CloseInfo();
            return;
        }

        // Tắt info của các slot khác
        for (int i = 0; i < displays.Length; i++)
            if (i != selected) displays[i]?.ShowInfo(false);

        // Toggle slot đang chọn
        var d = displays[selected];
        d?.ToggleInfo();

        // Đồng bộ trạng thái popup/dimmer theo UI thực tế (không dựa vào flag lật tay)
        isInfoOpen = d != null && d.infoPanel != null && d.infoPanel.activeSelf;
        SetPopup(isInfoOpen);
    }

    void CloseInfo()
    {
        for (int i = 0; i < displays.Length; i++)
            displays[i]?.ShowInfo(false);

        isInfoOpen = false;
        SetPopup(false);
    }

    void SetPopup(bool show)
    {
        if (dimmer)    dimmer.SetActive(show);
        if (infoPanel) infoPanel.SetActive(show);

        if (show && infoPanel)
        {
            // đảm bảo InfoPanel ở giữa & trên cùng
            var rt = infoPanel.GetComponent<RectTransform>();
            if (rt)
            {
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;
            }
            if (dimmer) dimmer.transform.SetAsLastSibling();
            infoPanel.transform.SetAsLastSibling();
        }
    }

    // ================ LOGIC GỐC ================
    void RandomizeOffers()
    {
        var pool = new List<ItemSO>(catalog.Where(x => x != null));
        for (int i = 0; i < 3; i++)
        {
            slots[i].sold = false;
            if (pool.Count > 0)
            {
                int idx = Random.Range(0, pool.Count);
                slots[i].item = pool[idx];
                pool.RemoveAt(idx);
            }
            else slots[i].item = null;
        }
        selected = 0;
    }

    void RefreshDisplays()
    {
        for (int i = 0; i < displays.Length; i++)
        {
            var d = displays[i];
            if (!d) continue;
            d.Show(slots[i].item);
            d.SetSold(slots[i].sold);
            d.ShowInfo(false); // ẩn info mặc định
            d.SetHighlight(i == selected);
        }
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

        if (item is PermanentStatSO perm)      perm.ApplyTo(pstats);
        else if (item is Consumable con)       con.UseOn(pstats);

        s.sold = true;
        slots[index] = s;
        displays[index]?.SetSold(true);
        return true;
    }

    // ——— được ShopSlotTrigger gọi ———
    public void SetPlayer(PlayerStats ps, CurrencyWallet w)
    {
        pstats = ps;
        wallet = w;
        isOpen = true;
    }

    public void SetSelectedSlot(int index)
    {
        selected = Mathf.Clamp(index, 0, displays.Length - 1);
        for (int i = 0; i < displays.Length; i++)
            displays[i]?.SetHighlight(i == selected);
    }

    public void OnLeaveSlot(int idx, PlayerStats ps)
    {
        if (idx == selected) displays[idx]?.SetHighlight(false);
        // có thể CloseInfo(); nếu muốn rời slot là đóng popup
    }
}

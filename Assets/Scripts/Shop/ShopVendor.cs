using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ShopSlot
{
    [HideInInspector] public ItemSO item;
    [HideInInspector] public bool sold;
}

/// <summary>
/// Quản lý shop: random item, hiển thị Display, mua bằng phím B,
/// và hiển thị tooltip mô tả cho slot đang đứng.
/// </summary>
public class ShopVendor : MonoBehaviour
{
    [Header("Catalog")]
    public List<ItemSO> catalog = new List<ItemSO>();

    [Header("Displays (kéo 3 ShopItemDisplay của Slot0/1/2)")]
    public ShopItemDisplay[] displays = new ShopItemDisplay[3];

    [Header("Tooltip UI")]
    public ShopTooltipUI tooltip; // kéo Tooltip (có ShopTooltipUI) vào đây

    [Header("Input")]
    public KeyCode buyKey = KeyCode.B;

    [Header("State")]
    public ShopSlot[] slots = new ShopSlot[3] { new(), new(), new() };
    int selected = 0;
    bool isOpen = true;

    CurrencyWallet wallet;
    PlayerStats pstats;

    void Start()
    {
        if (catalog == null || catalog.Count == 0)
            Debug.LogWarning("ShopVendor: catalog rỗng!");

        RandomizeOffers();
        RefreshDisplays();

        if (tooltip) tooltip.Hide(true);
    }

    void Update()
    {
        if (!isOpen) return;

        // Mua
        if (Input.GetKeyDown(buyKey))
        {
            if (TryBuy(selected))
            {
                // Mua xong thì ẩn tooltip (nếu item đã bán)
                if (tooltip) tooltip.Hide();
            }
        }
    }

    // ====== Tooltip / Trigger API (được gọi từ ShopSlotTrigger) ======

    /// <summary>Gọi khi Player bước vào vùng slot.</summary>
    public void OnEnterSlot(int index, Transform worldAnchor, PlayerStats ps, CurrencyWallet w)
    {
        pstats = ps;
        wallet = w;

        SetSelectedSlot(index);

        var s = slots[selected];
        if (s.item != null && !s.sold && tooltip)
        {
            tooltip.ShowAt(worldAnchor != null ? worldAnchor : displays[selected].transform,
                           s.item.description);
        }
    }

    /// <summary>Gọi liên tục khi đứng trong vùng slot (giữ highlight đúng).</summary>
    public void OnStaySlot(int index)
    {
        SetSelectedSlot(index);
    }

    /// <summary>Gọi khi Player rời slot.</summary>
    public void OnExitSlot(int index)
    {
        if (index == selected && tooltip) tooltip.Hide();
        // Không tắt highlight ở đây nếu bạn còn đứng ở slot khác;
        // highlight sẽ được set ở SetSelectedSlot khi OnEnter slot khác.
    }

    // ====== Core shop logic ======

    void RandomizeOffers()
    {
        var pool = new List<ItemSO>(catalog.Where(x => x != null));
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].sold = false;
            if (pool.Count > 0)
            {
                int idx = Random.Range(0, pool.Count);
                slots[i].item = pool[idx];
                pool.RemoveAt(idx);
            }
            else
            {
                slots[i].item = null;
            }
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

        // Áp dụng item
        if (item is PermanentStatSO perm)      perm.ApplyTo(pstats);
        else if (item is Consumable con)       con.UseOn(pstats);

        s.sold = true;
        slots[index] = s;

        // Cập nhật Display
        if (index >= 0 && index < displays.Length)
            displays[index]?.SetSold(true);

        return true;
    }

    public void SetSelectedSlot(int index)
    {
        selected = Mathf.Clamp(index, 0, displays.Length - 1);

        for (int i = 0; i < displays.Length; i++)
            displays[i]?.SetHighlight(i == selected);
    }
}

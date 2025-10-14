using UnityEngine;

public class SimpleShopUI : MonoBehaviour
{
    public ShopVendor vendor;  // gán shop cần mở
    public GameObject player;  // người chơi
    bool isOpen = false;
    int selectedIndex = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            isOpen = !isOpen;
            selectedIndex = 0;
            Debug.Log(isOpen ? "📦 SHOP OPENED" : "📦 SHOP CLOSED");
        }

        if (!isOpen || vendor == null) return;

        // di chuyển con trỏ
        if (Input.GetKeyDown(KeyCode.UpArrow)) selectedIndex = Mathf.Max(0, selectedIndex - 1);
        if (Input.GetKeyDown(KeyCode.DownArrow)) selectedIndex = Mathf.Min(vendor.goods.Length - 1, selectedIndex + 1);

        // hiển thị item hiện tại
        var entry = vendor.goods[selectedIndex];
        Debug.Log($"Đang chọn: {entry.item.displayName} - {entry.price} vàng");

        // mua
        if (Input.GetKeyDown(KeyCode.Return))
        {
            vendor.TryBuy(selectedIndex);
        }
    }
}

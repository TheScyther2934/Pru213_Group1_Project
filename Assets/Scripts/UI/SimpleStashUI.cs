using UnityEngine;

public class SimpleStashUI : MonoBehaviour
{
    public Stash stash;
    public PlayerStats player;
    bool isOpen = false;
    int selectedIndex = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isOpen = !isOpen;
            selectedIndex = 0;
            Debug.Log(isOpen ? "🎒 STASH OPENED" : "🎒 STASH CLOSED");
        }

        if (!isOpen || stash == null) return;

        if (stash.slots.Count == 0)
        {
            Debug.Log("Rương trống.");
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) selectedIndex = Mathf.Max(0, selectedIndex - 1);
        if (Input.GetKeyDown(KeyCode.DownArrow)) selectedIndex = Mathf.Min(stash.slots.Count - 1, selectedIndex + 1);

        var slot = stash.slots[selectedIndex];
        Debug.Log($"Đang chọn: {slot.item.displayName} x{slot.quantity}");

        if (Input.GetKeyDown(KeyCode.Return))
        {
            stash.UseAt(selectedIndex, player);
            Debug.Log($"Đã dùng {slot.item.displayName}");
        }
    }
}

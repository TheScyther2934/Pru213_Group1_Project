using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StashSlot { public ItemSO item; public int quantity = 1; }

public class Stash : MonoBehaviour
{
    public List<StashSlot> slots = new();

    public void Add(ItemSO item, int amount = 1)
    {
        if (!item || amount <= 0) return;
        if (item.stackable)
        {
            var s = slots.Find(x => x.item == item);
            if (s != null) { s.quantity += amount; return; }
        }
        slots.Add(new StashSlot { item = item, quantity = amount });
    }

    public bool UseAt(int index, PlayerStats target)
    {
        if (index < 0 || index >= slots.Count) return false;
        var s = slots[index];
        if (!s.item) return false;

        // gọi theo loại
        if (s.item is TimedBuffSO tb) tb.UseOn(target);
        else if (s.item is PermanentStatSO ps) ps.UseOn(target);
        else return false;

        s.quantity--;
        if (s.quantity <= 0) slots.RemoveAt(index); else slots[index] = s;
        return true;
    }
}

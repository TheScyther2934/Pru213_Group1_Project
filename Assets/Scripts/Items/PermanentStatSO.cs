using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Items/Permanent Stat")]
public class PermanentStatSO : ItemSO
{
    [Header("Permanent Bonuses")]
    public List<StatModifier> bonuses; // duration <= 0 (vĩnh viễn)

    private void Reset()
    {
        kind = ItemKind.PermanentStat;
        stackable = true; // tuỳ; có thể false nếu chỉ cho dùng 1 lần
    }

    public void UseOn(PlayerStats target)
    {
        if (!target) return;
        // đảm bảo là vĩnh viễn
        for (int i = 0; i < bonuses.Count; i++)
        {
            var m = bonuses[i];
            m.duration = 0f;
            bonuses[i] = m;
        }
        target.AddModifiers(bonuses);
    }
}

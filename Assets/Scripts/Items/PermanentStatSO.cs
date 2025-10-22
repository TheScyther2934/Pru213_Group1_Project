using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Items/Permanent Stat")]
public class PermanentStatSO : ItemSO
{
    [Header("Permanent Bonuses")]
    public List<StatModifier> bonuses; // duration <= 0

    private void Reset() { kind = ItemKind.PermanentStat; stackable = true; }

    public void ApplyTo(PlayerStats target) {
        if (!target) return;
        // đảm bảo vĩnh viễn
        for (int i = 0; i < bonuses.Count; i++) {
            var m = bonuses[i];
            m.duration = 0f;
            bonuses[i] = m;
        }
        target.AddModifiers(bonuses);
    }
}

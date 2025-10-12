using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Consumable")]
public class Consumable : ScriptableObject
{
    public string displayName;
    [TextArea] public string description;

    // (Tuỳ chọn) icon từ bộ sprite bạn có
    public Sprite icon;

    // Các hiệu ứng chỉ số (buff/debuff)
    public List<StatModifier> effects;

    // (Tuỳ chọn) hồi máu ngay khi dùng
    public float healAmount;

    public void UseOn(PlayerStats target)
    {
        if (!target) return;

        if (effects != null && effects.Count > 0)
            target.AddModifiers(effects);

        if (healAmount > 0f)
            target.Heal(healAmount);
    }
}

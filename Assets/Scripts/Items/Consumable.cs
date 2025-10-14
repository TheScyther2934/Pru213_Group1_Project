using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Consumable")]
public class Consumable : ItemSO
{
    [Header("Effects")]
    public List<StatModifier> effects;
    public float healAmount;

    public void UseOn(PlayerStats target)
    {
        if (!target) return;
        if (effects != null && effects.Count > 0) target.AddModifiers(effects);
        if (healAmount > 0f) target.Heal(healAmount);
    }
}

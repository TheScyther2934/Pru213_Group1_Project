using System;
using System.Collections.Generic;

public enum ModifierMode { Flat, PercentAdd, PercentMult } // +10, +10%, *1.1

[Serializable]
public struct StatModifier
{
    public StatType stat;
    public ModifierMode mode;
    public float value;
    public float duration; // <=0 = vĩnh viễn
    [NonSerialized] public float timeLeft; // runtime
}

[Serializable]
public class Stat
{
    public StatType type;
    public float baseValue = 0f;
    public List<StatModifier> modifiers = new();

    public float GetFinalValue()
    {
        float v = baseValue;
        float percentAdd = 0f;
        float percentMult = 1f;

        foreach (var m in modifiers)
        {
            switch (m.mode)
            {
                case ModifierMode.Flat: v += m.value; break;
                case ModifierMode.PercentAdd: percentAdd += m.value; break;   // 0.10 = +10%
                case ModifierMode.PercentMult: percentMult *= (1f + m.value); break;
            }
        }
        v *= (1f + percentAdd);
        v *= percentMult;
        return v;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats (khai trong Inspector)")]
    public List<Stat> stats = new List<Stat>();

    [Header("Runtime")]
    public float currentHealth;

    [Header("Events")]
    public UnityEvent OnDeath;
    public UnityEvent<float> OnDamaged;
    public UnityEvent<float> OnHealed;

    Dictionary<StatType, Stat> map;

    void Awake()
    {
        map = new Dictionary<StatType, Stat>();
        foreach (var s in stats) map[s.type] = s;

        float maxHp = Get(StatType.MaxHealth);
        currentHealth = (currentHealth <= 0f)
            ? maxHp
            : Mathf.Clamp(currentHealth, 0f, maxHp);
        foreach (var s in stats)
            for (int i = 0; i < s.modifiers.Count; i++)
                if (s.modifiers[i].duration > 0f && s.modifiers[i].timeLeft <= 0f)
                {
                    var m = s.modifiers[i];
                    m.timeLeft = m.duration;
                    s.modifiers[i] = m;
                }
    }


    void Update()
    {
        // đếm thời gian cho modifier có thời hạn
        foreach (var s in stats)
        {
            for (int i = s.modifiers.Count - 1; i >= 0; i--)
            {
                var m = s.modifiers[i];
                if (m.duration > 0f)
                {
                    m.timeLeft = (m.timeLeft <= 0f ? m.duration : m.timeLeft) - Time.deltaTime;
                    if (m.timeLeft <= 0f) s.modifiers.RemoveAt(i);
                    else s.modifiers[i] = m;
                }
            }
        }
        // giữ HP không vượt MaxHealth khi stat đổi
        currentHealth = Mathf.Min(currentHealth, Get(StatType.MaxHealth));
    }

    public float Get(StatType type)
    {
        return map.TryGetValue(type, out var s) ? s.GetFinalValue() : 0f;
    }

    public void AddModifier(StatModifier mod)
    {
        if (!map.TryGetValue(mod.stat, out var s)) return;
        if (mod.duration > 0f) mod.timeLeft = mod.duration;
        s.modifiers.Add(mod);
    }

    public void AddModifiers(IEnumerable<StatModifier> mods)
    {
        foreach (var m in mods) AddModifier(m);
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, Get(StatType.MaxHealth));
        OnHealed?.Invoke(amount);
    }

    public void TakeDamage(float rawDamage)
    {
        float def = Get(StatType.Defense);
        float dmg = Mathf.Max(1f, rawDamage - def);
        currentHealth -= dmg;
        OnDamaged?.Invoke(dmg);
        if (currentHealth <= 0f) OnDeath?.Invoke();
    }

    public float RollAttackDamage()
    {
        float atk = Get(StatType.AttackPower);
        float critChance = Mathf.Clamp01(Get(StatType.CritChance));
        float critMul = Mathf.Max(1f, Get(StatType.CritMultiplier));
        bool isCrit = Random.value < critChance;
        return isCrit ? atk * critMul : atk;
    }
#if UNITY_EDITOR
void Reset() {
    stats = new System.Collections.Generic.List<Stat> {
        new Stat { type = StatType.MaxHealth,     baseValue = 100 },
        new Stat { type = StatType.AttackPower,   baseValue = 10  },
        new Stat { type = StatType.Defense,       baseValue = 2   },
        new Stat { type = StatType.MoveSpeed,     baseValue = 5   },
        new Stat { type = StatType.CritChance,    baseValue = 0.1f},
        new Stat { type = StatType.CritMultiplier,baseValue = 1.5f}
    };
    currentHealth = 0;
}
#endif

}

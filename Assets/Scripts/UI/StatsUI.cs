using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsUI : MonoBehaviour
{
    public PlayerStats playerStats;
    public Slider slider;
    public TextMeshProUGUI healthText; // 👈 add this line
    public TextMeshProUGUI statsText;
    void Start()
    {
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        slider.maxValue = playerStats.Get(StatType.MaxHealth);
        slider.value = playerStats.currentHealth;

        UpdateHealthText();
        UpdateStats();

        // Subscribe to player events
        playerStats.OnDamaged.AddListener(UpdateHealth);
        playerStats.OnHealed.AddListener(UpdateHealth);
        playerStats.OnDeath.AddListener(OnPlayerDeath);
    }

    void Update()
    {
        slider.maxValue = playerStats.Get(StatType.MaxHealth);
        slider.value = playerStats.currentHealth;
        UpdateHealthText();
    }

    void UpdateHealth(float _)
    {
        slider.value = playerStats.currentHealth;
        UpdateHealthText();
    }

    void UpdateStats()
    {
        if (playerStats == null || statsText == null) return;

        float atk = playerStats.Get(StatType.AttackPower);
        float def = playerStats.Get(StatType.Defense);
        float spd = playerStats.Get(StatType.MoveSpeed);
        float critChance = playerStats.Get(StatType.CritChance) * 100f;
        float critMul = playerStats.Get(StatType.CritMultiplier);

        statsText.text =
            $"Attack: {atk}\n" +
            $"Defense: {def}\n" +
            $"Speed: {spd}\n" +
            $"Crit: {critChance:F0}% (x{critMul:F1})";
    }
    void UpdateHealthText()
    {
        float current = Mathf.Round(playerStats.currentHealth);
        float max = Mathf.Round(playerStats.Get(StatType.MaxHealth));
        healthText.text = $"{current} / {max}";
    }

    void OnPlayerDeath()
    {
        slider.value = 0;
        UpdateHealthText();
    }
}

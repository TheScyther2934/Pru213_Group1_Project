using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    public PlayerStats playerStats;
    public Slider slider;
    public TextMeshProUGUI healthText; // 👈 add this line

    void Start()
    {
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        slider.maxValue = playerStats.Get(StatType.MaxHealth);
        slider.value = playerStats.currentHealth;

        UpdateHealthText();

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

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsUI : MonoBehaviour
{
    public PlayerStats playerStats;
    public Slider slider;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI statsText;

    // cache để chỉ update khi thật sự đổi
    float lastAtk, lastDef, lastSpd, lastCritC, lastCritM;
    float pollTimer = 0f;
    const float POLL_INTERVAL = 0.2f; // làm tươi mỗi 0.2s để đỡ tốn

    void Start()
    {
        if (!playerStats) playerStats = FindObjectOfType<PlayerStats>();
        if (!playerStats || !slider) return;

        slider.maxValue = playerStats.Get(StatType.MaxHealth);
        slider.value    = playerStats.currentHealth;

        UpdateHealthText();
        ForceUpdateStats(); // khởi tạo statsText ngay

        // đăng ký sự kiện HP
        playerStats.OnDamaged.AddListener(UpdateHealth);
        playerStats.OnHealed.AddListener(UpdateHealth);
        playerStats.OnDeath.AddListener(OnPlayerDeath);
    }

    void OnDestroy()
    {
        if (!playerStats) return;
        playerStats.OnDamaged.RemoveListener(UpdateHealth);
        playerStats.OnHealed.RemoveListener(UpdateHealth);
        playerStats.OnDeath.RemoveListener(OnPlayerDeath);
    }

    void Update()
    {
        if (!playerStats || !slider) return;

        // thanh máu + text máu
        slider.maxValue = playerStats.Get(StatType.MaxHealth);
        slider.value    = playerStats.currentHealth;
        UpdateHealthText();

        // làm tươi statsText định kỳ hoặc khi thay đổi
        pollTimer += Time.unscaledDeltaTime;
        if (pollTimer >= POLL_INTERVAL)
        {
            pollTimer = 0f;

            float atk = playerStats.Get(StatType.AttackPower);
            float def = playerStats.Get(StatType.Defense);
            float spd = playerStats.Get(StatType.MoveSpeed);
            float cc  = playerStats.Get(StatType.CritChance);
            float cm  = playerStats.Get(StatType.CritMultiplier);

            // so sánh có đổi không (dùng sai số nhỏ để tránh nhấp nháy)
            if (!Approximately(atk, lastAtk) || !Approximately(def, lastDef) ||
                !Approximately(spd, lastSpd) || !Approximately(cc, lastCritC) ||
                !Approximately(cm, lastCritM))
            {
                UpdateStats(atk, def, spd, cc, cm);

                lastAtk   = atk;
                lastDef   = def;
                lastSpd   = spd;
                lastCritC = cc;
                lastCritM = cm;
            }
        }
    }

    bool Approximately(float a, float b) => Mathf.Abs(a - b) > 0.0001f;

    void UpdateHealth(float _)
    {
        if (!playerStats || !slider) return;
        slider.value = playerStats.currentHealth;
        UpdateHealthText();
    }

    void UpdateHealthText()
    {
        if (!healthText || !playerStats) return;
        float current = Mathf.Round(playerStats.currentHealth);
        float max     = Mathf.Round(playerStats.Get(StatType.MaxHealth));
        healthText.text = $"{current} / {max}";
    }

    // cập nhật stats khi đã biết giá trị mới
    void UpdateStats(float atk, float def, float spd, float critChance, float critMul)
    {
        if (!statsText) return;
        statsText.text =
            $"Attack: {atk}\n" +
            $"Defense: {def}\n" +
            $"Speed: {spd}\n" +
            $"Crit: {(critChance * 100f):F0}% (x{critMul:F1})";
    }

    // ép cập nhật ngay lần đầu
    void ForceUpdateStats()
    {
        lastAtk = lastDef = lastSpd = lastCritC = lastCritM = float.NaN;
        pollTimer = POLL_INTERVAL;
    }

    void OnPlayerDeath()
    {
        if (slider) slider.value = 0;
        UpdateHealthText();
    }
}

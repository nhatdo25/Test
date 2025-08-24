using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("Health Bar")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    [Header("Mana Bar")]
    public Slider manaSlider;
    public TextMeshProUGUI manaText;
    [Header("Experience & Level")]
    public Slider expSlider;
    public TextMeshProUGUI expText; // Text hiển thị % hoặc số EXP
    public TextMeshProUGUI levelText; // Text hiển thị cấp độ

    // Đăng ký lắng nghe khi UI được bật
    private void OnEnable()
    {
        PlayerStats.OnHealthChanged += UpdateHealth;
        PlayerStats.OnManaChanged += UpdateMana;
        PlayerStats.OnExperienceChanged += UpdateExperience;
        PlayerStats.OnPlayerLevelUp += UpdateLevelText;
    }

    // Hủy đăng ký khi UI bị tắt
    private void OnDisable()
    {
        PlayerStats.OnHealthChanged -= UpdateHealth;
        PlayerStats.OnManaChanged -= UpdateMana;
        PlayerStats.OnExperienceChanged -= UpdateExperience;
        PlayerStats.OnPlayerLevelUp -= UpdateLevelText;
    }
    void Start()
    {
        // Tự động tìm PlayerStats trong scene khi bắt đầu
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            // "Kéo" dữ liệu ban đầu về để hiển thị ngay lập tức
            //UpdateHealth(playerStats.GetCurrentHealth(), playerStats.GetMaxHealth());
            //UpdateMana(playerStats.GetCurrentMana(), playerStats.GetMaxMana());
            UpdateExperience(playerStats.exp, playerStats.expToNextLevel);
            UpdateLevelText(playerStats.level);
        }
    }
    void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        if (healthText != null)
        {
            healthText.text = $"{Mathf.RoundToInt(currentHealth)} / {Mathf.RoundToInt(maxHealth)}";
        }
    }

    void UpdateMana(float currentMana, float maxMana)
    {
        manaSlider.maxValue = maxMana;
        manaSlider.value = currentMana;
        if (manaText != null)
        {
            manaText.text = $"{Mathf.RoundToInt(currentMana)} / {Mathf.RoundToInt(maxMana)}";
        }
    }
    // Hàm này được gọi mỗi khi EXP thay đổi
    void UpdateExperience(int currentExp, int requiredExp)
    {
        if (expSlider != null)
        {
            expSlider.maxValue = requiredExp;
            expSlider.value = currentExp;
        }
        if (expText != null)
        {
            expText.text = $"{currentExp} / {requiredExp}";
        }
    }

    // Hàm này được gọi mỗi khi người chơi lên cấp
    void UpdateLevelText(int newLevel)
    {
        if (levelText != null)
        {
            levelText.text = $"Lv. {newLevel}";
        }
    }
}
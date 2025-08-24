using System.Collections.Generic;
using UnityEngine;

// Một lớp nhỏ để lưu trữ thông tin về một hiệu ứng đang hoạt động
public class ActiveHealthEffect
{
    public float PercentAmount; // % máu thay đổi mỗi giây
    public float DurationRemaining; // Thời gian còn lại
    private float tickTimer; // Bộ đếm để áp dụng hiệu ứng mỗi giây

    public bool Update(float deltaTime)
    {
        DurationRemaining -= deltaTime;
        tickTimer -= deltaTime;
        return tickTimer <= 0; // Trả về true nếu đã đến lúc áp dụng hiệu ứng
    }

    public void ResetTick()
    {
        tickTimer = 1.0f; // Đặt lại bộ đếm là 1 giây
    }
}

public class PlayerStatusEffects : MonoBehaviour
{
    private PlayerStats playerStats;
    private readonly List<ActiveHealthEffect> activeEffects = new List<ActiveHealthEffect>();

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        // Dùng vòng lặp ngược để có thể xóa item khỏi list một cách an toàn
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = activeEffects[i];

            // Nếu đã đến lúc áp dụng hiệu ứng (mỗi giây)
            if (effect.Update(Time.deltaTime))
            {
                ApplyEffect(effect);
                effect.ResetTick();
            }

            // Nếu hiệu ứng đã hết thời gian
            if (effect.DurationRemaining <= 0)
            {
                activeEffects.RemoveAt(i);
            }
        }
    }

    // Hàm để thêm một hiệu ứng mới từ bên ngoài
    public void AddEffect(StatusEffectData statusEffectData)
    {
        if (statusEffectData.duration <= 0) return;

        var newEffect = new ActiveHealthEffect
        {
            PercentAmount = statusEffectData.healthChangePercent,
            DurationRemaining = statusEffectData.duration
        };
        newEffect.ResetTick(); // Đặt tick ban đầu
        activeEffects.Add(newEffect);

        Debug.Log($"Áp dụng hiệu ứng: {statusEffectData.effectName} trong {statusEffectData.duration} giây.");
    }

    private void ApplyEffect(ActiveHealthEffect effect)
    {
        if (playerStats == null) return;

        // 1. Lấy maxHP thông qua hàm public một cách an toàn
        float maxHealth = playerStats.GetMaxHealth();

        // 2. Tính toán lượng máu thay đổi
        int healthChange = (int)(maxHealth * (effect.PercentAmount / 100f));

        // 3. "Nhờ" PlayerStats tự thay đổi máu thông qua hàm public
        playerStats.ChangeHealth(healthChange);
    }
}
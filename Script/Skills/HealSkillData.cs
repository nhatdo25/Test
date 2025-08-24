
using UnityEngine;

public enum ResourceToHeal { HP, Mana }
public enum HealType { InstantFlat, InstantPercent, OverTimeFlat, OverTimePercent }

[CreateAssetMenu(menuName = "Skills/Heal Skill")]
public class HealSkillData : SkillData
{
    [Header("Heal Settings")]
    public ResourceToHeal resourceType;
    public HealType healType;
    public float healValue; // Có thể là số lượng (200 HP) hoặc phần trăm (20%)
    public float duration = 5f; // Chỉ dùng cho các loại OverTime

    public override void Activate(GameObject user)
    {
        PlayerStats stats = user.GetComponent<PlayerStats>();
        if (stats == null) return;

        switch (healType)
        {
            case HealType.InstantFlat:
                HealResource(stats, (int)healValue);
                Debug.Log($"Healed {(int)healValue} {resourceType} instantly.");
                break;

            case HealType.InstantPercent:
                int maxResource = (resourceType == ResourceToHeal.HP) ? stats.currentStats.maxHP : stats.currentStats.maxMana;
                int amountToHeal = (int)(maxResource * (healValue / 100f));
                HealResource(stats, amountToHeal);
                Debug.Log($"Healed {healValue}% ({amountToHeal}) {resourceType} instantly.");
                break;

            // Đối với các loại OverTime, chúng ta sẽ giao nhiệm vụ cho BuffManager
            case HealType.OverTimeFlat:
            case HealType.OverTimePercent:
                var buffManager = user.GetComponent<BuffManager>();
                if (buffManager != null)
                {
                    buffManager.ApplyHealOverTime(this);
                    Debug.Log($"Applied Heal Over Time skill: {skillName}");
                }
                break;
        }
    }

    // Hàm tiện ích để hồi máu/mana
    private void HealResource(PlayerStats stats, int amount)
    {
        if (resourceType == ResourceToHeal.HP)
        {
            stats.currentStats.HP = Mathf.Min(stats.currentStats.HP + amount, stats.currentStats.maxHP);
        }
        else
        {
            stats.currentStats.Mana = Mathf.Min(stats.currentStats.Mana + amount, stats.currentStats.maxMana);
        }
    }
}
// BuffManager.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Lớp này không thay đổi
public class ActiveBuff
{
    public BuffSkillData Data;
    public float RemainingTime;
}

public class BuffManager : MonoBehaviour
{
    private PlayerStats playerStats;

    private readonly List<BuffSkillData> permanentBuffs = new List<BuffSkillData>();
    private readonly List<ActiveBuff> temporaryBuffs = new List<ActiveBuff>();

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        ProcessTemporaryBuffs();
    }

    // NÂNG CẤP: XỬ LÝ BUFF CỘNG DỒN
    public void ApplyBuff(BuffSkillData buff)
    {
        if (buff.isPermanent)
        {
            // Buff vĩnh viễn chỉ cần thêm một lần
            if (!permanentBuffs.Contains(buff))
            {
                permanentBuffs.Add(buff);
            }
        }
        else
        {
            // Kiểm tra xem buff tạm thời đã tồn tại chưa
            ActiveBuff existingBuff = temporaryBuffs.FirstOrDefault(b => b.Data == buff);
            if (existingBuff != null)
            {
                // Nếu có, làm mới thời gian
                existingBuff.RemainingTime = buff.duration;
                Debug.Log($"Refreshed buff: {buff.skillName}");
            }
            else
            {
                // Nếu không, thêm buff mới
                temporaryBuffs.Add(new ActiveBuff { Data = buff, RemainingTime = buff.duration });
            }
        }
        playerStats.CalculateFinalStats();
    }

    public void ApplyHealOverTime(HealSkillData skill)
    {
        StartCoroutine(HealOverTimeCoroutine(skill));
    }

    public float GetStatBonus(StatToBuff statType)
    {
        float bonus = 0;
        bonus += permanentBuffs.Where(b => b.statToBuff == statType).Sum(b => b.buffValue);
        bonus += temporaryBuffs.Where(b => b.Data.statToBuff == statType).Sum(b => b.Data.buffValue);
        return bonus;
    }

    private void ProcessTemporaryBuffs()
    {
        // ... code này đã tốt, không cần sửa ...
    }

    // SỬA LỖI: HỒI MÁU THEO THỜI GIAN
    private IEnumerator HealOverTimeCoroutine(HealSkillData skill)
    {
        float elapsedTime = 0f;
        float totalAmountToHeal = 0;
        float healAccumulator = 0f; // Biến tích lũy lượng máu lẻ

        // Tính tổng lượng máu cần hồi (logic này đã đúng)
        if (skill.healType == HealType.OverTimeFlat)
        {
            totalAmountToHeal = skill.healValue;
        }
        else // OverTimePercent
        {
            int maxResource = (skill.resourceType == ResourceToHeal.HP)
                                ? playerStats.currentStats.maxHP
                                : playerStats.currentStats.maxMana;
            totalAmountToHeal = maxResource * (skill.healValue / 100f);
        }

        float healPerSecond = totalAmountToHeal / skill.duration;

        while (elapsedTime < skill.duration)
        {
            // Tích lũy lượng máu cần hồi trong frame này
            healAccumulator += healPerSecond * Time.deltaTime;

            // Nếu tích lũy được hơn 1 đơn vị, hãy hồi máu và trừ đi
            if (healAccumulator >= 1f)
            {
                int amountToHealThisTick = Mathf.FloorToInt(healAccumulator);

                if (skill.resourceType == ResourceToHeal.HP)
                {
                    playerStats.currentStats.HP = Mathf.Min(playerStats.currentStats.HP + amountToHealThisTick, playerStats.currentStats.maxHP);
                }
                else
                {
                    playerStats.currentStats.Mana = Mathf.Min(playerStats.currentStats.Mana + amountToHealThisTick, playerStats.currentStats.maxMana);
                }

                healAccumulator -= amountToHealThisTick;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Debug.Log($"Heal Over Time skill '{skill.skillName}' has finished.");
    }
}
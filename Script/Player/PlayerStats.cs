using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour
{
    private BuffManager buffManager;
    [Header("Stats")]
    private CharacterStats baseStats;
    private CharacterStats initialClassStats;
    public CharacterStats currentStats;

    [Header("Leveling & Points")]
    public int availableStatPoints = 5;
    public int skillPoints = 5;
    public int exp;
    public int expToNextLevel = 100;
    public int level = 1;

    // --- EVENTS ---
    public static event Action<float, float> OnHealthChanged;
    public static event Action<float, float> OnManaChanged;
    public static event Action<int> OnStatPointsChanged;
    public static event Action<int> OnSkillPointsChanged;
    public static event Action<CharacterStats> OnStatsCalculated;
    public static event Action<int> OnPlayerLevelUp; // Thông báo cấp độ mới
    public static event Action<int, int> OnExperienceChanged; // currentExp, requiredExp
    //
    private bool isInitialized = false;
    void Awake()
    {
        currentStats = new CharacterStats();
        initialClassStats = new CharacterStats();
        baseStats = new CharacterStats();
        buffManager = GetComponent<BuffManager>();
    }

    // Hàm này được GameSceneManager gọi để thiết lập chỉ số ban đầu
    public void InitializeStats(CharacterStats classStats)
    {
        if (isInitialized) return;
        isInitialized = true;

        initialClassStats.CopyFrom(classStats);
        baseStats.CopyFrom(classStats);

        CalculateFinalStats();
        currentStats.HP = currentStats.maxHP;
        currentStats.Mana = currentStats.maxMana;

        UpdateUI();
        OnStatPointsChanged?.Invoke(availableStatPoints);
        OnSkillPointsChanged?.Invoke(skillPoints);
        // Phát tín hiệu trạng thái EXP ban đầu
        OnExperienceChanged?.Invoke(exp, expToNextLevel);
    }

    void Start()
    {
        EquipmentManager.OnEquipmentChanged += CalculateFinalStats;
    }

    private void OnDisable()
    {
        EquipmentManager.OnEquipmentChanged -= CalculateFinalStats;
    }

    private void UpdateUI()
    {
        // Chỉ phát tín hiệu nếu đã được khởi tạo
        if (isInitialized)
        {
            OnHealthChanged?.Invoke(currentStats.HP, currentStats.maxHP);
            OnManaChanged?.Invoke(currentStats.Mana, currentStats.maxMana);
        }
    }
    //Hàm đa năng để thay đổi máu (hồi máu nếu amount > 0, nhận sát thương nếu amount < 0)
    public void ChangeHealth(int amount)
    {
        currentStats.HP += amount;
        currentStats.HP = Mathf.Clamp(currentStats.HP, 0, currentStats.maxHP);
        OnHealthChanged?.Invoke(currentStats.HP, currentStats.maxHP);
        Debug.Log($"Thay đổi {amount} HP, máu hiện tại: {currentStats.HP}/{currentStats.maxHP}");
    }
    //Hàm đa năng để thay đổi mana (hồi mana nếu amount > 0, nhận sát thương nếu amount < 0)
    public void ChangeMana(int amount)
    {
        currentStats.Mana += amount;
        currentStats.Mana = Mathf.Clamp(currentStats.Mana, 0, currentStats.maxMana);
        OnManaChanged?.Invoke(currentStats.Mana, currentStats.maxMana);
        Debug.Log($"Thay đổi {amount} Mana, mana hiện tại: {currentStats.Mana}/{currentStats.maxMana}");
    }
    //Hàm nhận sát thương
    public void TakeDamage(int damage)
    {
        // (logic tính toán sát thương dựa trên giáp ở đây)
        // int finalDamage = damage - currentStats.ARP;
        // if (finalDamage < 1) finalDamage = 1;
        
        ChangeHealth(-damage); // Dùng ChangeHealth với số âm
    }
    //Hàm hồi máu
    public void Heal(int amount)
    {
        ChangeHealth(amount); // Dùng ChangeHealth với số dương
    }
    //Hàm tính toán chỉ số cuối cùng
    public void CalculateFinalStats()
    {
        if (!isInitialized) return;
        // 1. Lưu lại tỷ lệ % máu và mana hiện tại
        // (Kiểm tra để tránh chia cho 0 nếu maxHP = 0 lúc đầu)
        float hpPercent = (currentStats.maxHP > 0) ? (float)currentStats.HP / currentStats.maxHP : 1f;
        float manaPercent = (currentStats.maxMana > 0) ? (float)currentStats.Mana / currentStats.maxMana : 1f;

        // 2. Reset currentStats về chỉ số gốc
        currentStats.CopyFrom(baseStats);

        // 3. Cộng dồn chỉ số từ trang bị
        if (EquipmentManager.instance != null)
        {
            List<EquipmentData> equippedItems = EquipmentManager.instance.GetCurrentEquipment();
            foreach (EquipmentData item in equippedItems)
            {
                currentStats.maxHP += item.maxHP;
                currentStats.maxMana += item.maxMana;
                currentStats.PHI += item.physicalDamage;
                currentStats.MAG += item.magicDamage;
                currentStats.ARP += item.physicalArmor;
                currentStats.ARM += item.magicArmor;
                currentStats.CRITR += item.critRate;
                currentStats.CRITD += item.critDamage;
            }
        }
        // 4. ÁP DỤNG CHỈ SỐ TỪ BUFF (VĨNH VIỄN VÀ TẠM THỜI)
        if (buffManager != null)
        {
            // Cộng các chỉ số cố định trước
            currentStats.ARM += (int)buffManager.GetStatBonus(StatToBuff.ARMOR);
            currentStats.maxHP += (int)buffManager.GetStatBonus(StatToBuff.MAX_HP);
            currentStats.maxMana += (int)buffManager.GetStatBonus(StatToBuff.MAX_MANA);

            // Cộng các chỉ số % sau
            float totalAtkPercent = buffManager.GetStatBonus(StatToBuff.ATK_PERCENT);
            float totalDefPercent = buffManager.GetStatBonus(StatToBuff.DEF_PERCENT);

            currentStats.ATK += totalAtkPercent;
            currentStats.DEF += totalDefPercent;
        }
        // 5. Khôi phục lại máu và mana hiện tại dựa trên tỷ lệ % đã lưu
        currentStats.HP = (int)(currentStats.maxHP * hpPercent);
        currentStats.Mana = (int)(currentStats.maxMana * manaPercent);

        // 6. Cập nhật lại UI với các chỉ số cuối cùng
        OnStatsCalculated?.Invoke(currentStats);
        UpdateUI();
    }

    // --- CÁC HÀM TÍNH SÁT THƯƠNG ---
    //Tính sát thương vật lý
    public int GetFinalPhysicalDamage()
    {
        int finalDamage = currentStats.PHI;
        float randomValue = UnityEngine.Random.value;
        if (randomValue <= currentStats.CRITR / 100f)
        {
            finalDamage = (int)(finalDamage * (1 + currentStats.CRITD / 100f));
            Debug.Log("CRITICAL HIT! Sát thương vật lý: " + finalDamage);
        }
        else
        {
            Debug.Log("Sát thương vật lý: " + finalDamage);
        }
        return finalDamage;
    }
    //Tính sát thương phép
    public int GetFinalMagicalDamage()
    {
        int finalDamage = currentStats.MAG;
        float randomValue = UnityEngine.Random.value;
        if (randomValue <= currentStats.CRITR / 100f)
        {
            finalDamage = (int)(finalDamage * (1 + currentStats.CRITD / 100f));
            Debug.Log("CRITICAL HIT! Sát thương phép: " + finalDamage);
        }
        else
        {
            Debug.Log("Sát thương phép: " + finalDamage);
        }
        return finalDamage;
    }
    //Cộng điểm vào chỉ số GỐC (baseStats)
    public void ApplyStatAllocation(CharacterStats pointsToAdd)
    {
        int totalPointsSpent = pointsToAdd.maxHP + pointsToAdd.maxMana +
                             pointsToAdd.PHI + pointsToAdd.MAG +
                             pointsToAdd.ARP + pointsToAdd.ARM +
                             (int)pointsToAdd.CRITR + (int)pointsToAdd.CRITD + (int)pointsToAdd.moveSpeed;

        if (totalPointsSpent > availableStatPoints) return;

        availableStatPoints -= totalPointsSpent;

        // Cộng điểm vào baseStats
        baseStats.maxHP += pointsToAdd.maxHP;
        baseStats.maxMana += pointsToAdd.maxMana;
        baseStats.PHI += pointsToAdd.PHI;
        baseStats.MAG += pointsToAdd.MAG;
        baseStats.ARP += pointsToAdd.ARP;
        baseStats.ARM += pointsToAdd.ARM;
        baseStats.CRITR += pointsToAdd.CRITR;
        baseStats.CRITD += pointsToAdd.CRITD;
        baseStats.moveSpeed += pointsToAdd.moveSpeed;

        OnStatPointsChanged?.Invoke(availableStatPoints);
        CalculateFinalStats();
    }
    public void ResetStats()
    {
        // Tính toán số điểm đã dùng bằng cách so sánh baseStats hiện tại và chỉ số gốc của class
        int pointsSpent = (baseStats.maxHP - initialClassStats.maxHP) +
                          (baseStats.maxMana - initialClassStats.maxMana) +
                          (baseStats.PHI - initialClassStats.PHI) +
                          (baseStats.MAG - initialClassStats.MAG) +
                          (baseStats.ARP - initialClassStats.ARP) +
                          (baseStats.ARM - initialClassStats.ARM) +
                          (int)(baseStats.CRITR - initialClassStats.CRITR) +
                          (int)(baseStats.CRITD - initialClassStats.CRITD) +
                          (int)(baseStats.moveSpeed - initialClassStats.moveSpeed);

        availableStatPoints += pointsSpent;
        baseStats.CopyFrom(initialClassStats); // Reset baseStats về trạng thái ban đầu của class

        OnStatPointsChanged?.Invoke(availableStatPoints);
        CalculateFinalStats();
    }
    public void AddStatPoints(int amount)
    {
        availableStatPoints += amount;
        Debug.Log($"Nhận được {amount} điểm. Tổng điểm: {availableStatPoints}");
        
        // --- PHÁT TÍN HIỆU ---
        OnStatPointsChanged?.Invoke(availableStatPoints);
    }
    //Lấy chỉ số gốc
    public CharacterStats GetBaseStats() => baseStats;
    public CharacterStats GetCurrentStats() => currentStats;
    public float GetMaxHealth() => currentStats.maxHP;
    public float GetCurrentMana() => currentStats.Mana;
    public float GetMaxMana() => currentStats.maxMana;
    // logic skill point
    public void AddSkillPoints(int amount)
    {
        skillPoints += amount;
        OnSkillPointsChanged?.Invoke(skillPoints);
        Debug.Log($"Nhận được {amount} điểm kỹ năng. Tổng điểm: {skillPoints}");
    }

    public void SpendSkillPoints(int amount)
    {
        skillPoints -= amount;
        OnSkillPointsChanged?.Invoke(skillPoints);
    }
    // HÀM ĐỂ NHẬN KINH NGHIỆM
    public void AddExperience(int expGained)
    {
        exp += expGained;
        Debug.Log($"Nhận được {expGained} kinh nghiệm. Tổng cộng: {exp}/{expToNextLevel}");
        OnExperienceChanged?.Invoke(exp, expToNextLevel);// Phát tín hiệu để thanh EXP cập nhật

        // Kiểm tra xem có lên cấp không
        while (exp >= expToNextLevel)
        {
            LevelUp();
        }
    }
    private void LevelUp()
    {
        exp -= expToNextLevel; // Trừ đi exp đã dùng
        level++;
        
        // Cập nhật mốc EXP cho cấp độ tiếp theo theo quy tắc
        expToNextLevel *= 2; 

        // Thưởng điểm
        availableStatPoints += 5; 
        skillPoints += 1;

        Debug.Log($"🎉 CHÚC MỪNG LÊN CẤP {level}! 🎉");
        
        // Phát tín hiệu lên cấp
        OnPlayerLevelUp?.Invoke(level);
        // Phát tín hiệu EXP mới
        OnExperienceChanged?.Invoke(exp, expToNextLevel);
        // Phát tín hiệu điểm mới
        OnStatPointsChanged?.Invoke(availableStatPoints);
        OnSkillPointsChanged?.Invoke(skillPoints);
    }

    private int CalculateNextLevelExp()
    {
        // Một công thức tính exp đơn giản
        return (int)(expToNextLevel * 1.5f);
    }
}

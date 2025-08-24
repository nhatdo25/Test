using System.Collections.Generic;
using System.Linq; // Thêm thư viện Linq để tối ưu hóa
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    // không chỉ quản lý cooldown mà cả cấp độ
    public class SkillRuntimeData
    {
        public float CooldownRemaining;
        public int CurrentLevel;
    }
    public List<SkillData> allPossibleSkills; // Danh sách tất cả skill có thể có
    public List<SkillData> CurrentClassSkills { get; private set; }    // Skill của class hiện tại
    private Dictionary<SkillData, SkillRuntimeData> skillData = new Dictionary<SkillData, SkillRuntimeData>();

    public List<SkillData> skills;
    private Dictionary<SkillData, float> skillCooldowns = new Dictionary<SkillData, float>();

    private PlayerStats playerStats;
    private Animator animator;
    private AnimatorOverrideController animatorOverrideController;

    private SkillData skillToActivate; // Lưu lại skill đang chuẩn bị được kích hoạt

    public static System.Action<SkillData> OnSkillCooldownChanged;
    public static System.Action<SkillData> OnSkillUsed;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        animator = GetComponent<Animator>();

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        foreach (var skill in skills)
        {
            skillCooldowns[skill] = 0f;
            if (skill.skillType == SkillType.Passive)
            {
                skill.Activate(gameObject);
            }
        }
    }
    public void Initialize(ClassDefinition classDef)
    {
        CurrentClassSkills = classDef.classSkills; 
        
        foreach (var skill in allPossibleSkills)
        {
            // Khởi tạo tất cả skill với level 0
            skillData[skill] = new SkillRuntimeData { CooldownRemaining = 0, CurrentLevel = 0 };
        }
    }
    void Update()
    {
        // Tối ưu hóa việc giảm cooldown
        foreach (var skill in skillCooldowns.Keys.ToList())
        {
            if (skillCooldowns[skill] > 0)
            {
                skillCooldowns[skill] -= Time.deltaTime;
            }
        }
    }

    // Hàm này chỉ kiểm tra và bắt đầu animation
    public void UseSkill(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= skills.Count) return;

        SkillData skill = skills[skillIndex];

        if (skill.skillType != SkillType.Active) return;
        if (skillCooldowns[skill] > 0)
        {
            Debug.Log($"Skill '{skill.skillName}' is on cooldown.");
            return;
        }
        if (playerStats.currentStats.Mana < skill.manaCost)
        {
            Debug.Log("Not enough mana!");
            return;
        }
        if (playerStats.currentStats.Mana < skill.manaCost)
{
    Debug.Log("Không đủ mana!");
    return;
}
        // Lưu skill này lại để Animation Event có thể gọi
        skillToActivate = skill;

        if (skill.skillAnimation != null)
        {
            animatorOverrideController["UseSkill"] = skill.skillAnimation;
            animator.SetTrigger("UseSkillTrigger");
        }
        else
        {
            // Nếu skill không có animation, kích hoạt hiệu ứng ngay lập tức
            AnimationTrigger();
        }
    }

    // HÀM NÀY SẼ ĐƯỢC GỌI BỞI ANIMATION EVENT
    public void AnimationTrigger()
    {
        if (skillToActivate == null) return;

        // Bây giờ mới trừ mana, kích hoạt hiệu ứng và tính cooldown
        playerStats.currentStats.Mana -= skillToActivate.manaCost;
        skillToActivate.Activate(gameObject);
        skillCooldowns[skillToActivate] = skillToActivate.cooldown;
        if (OnSkillUsed != null)
    {
    OnSkillUsed(skillToActivate);
    }
        // Reset lại để chờ skill tiếp theo
        skillToActivate = null;
    }
    //  Để nâng cấp kỹ năng
    public bool LevelUpSkill(SkillData skill)
    {
        PlayerStats stats = GetComponent<PlayerStats>();
        if (stats.skillPoints < skill.skillPointCost)
        {
            Debug.Log("Không đủ điểm kỹ năng!");
            return false;
        }

        if (skillData[skill].CurrentLevel >= skill.maxLevel)
        {
            Debug.Log("Kỹ năng đã đạt cấp tối đa!");
            return false;
        }

        stats.SpendSkillPoints(skill.skillPointCost);
        skillData[skill].CurrentLevel++;
        Debug.Log($"{skill.skillName} đã được nâng lên cấp {skillData[skill].CurrentLevel}");
        // TODO: Phát event để UI cập nhật
        return true;
    }
    //Để lấy cấp độ hiện tại của skill
    public int GetSkillLevel(SkillData skill)
    {
        return skillData.ContainsKey(skill) ? skillData[skill].CurrentLevel : 0;
    }
}
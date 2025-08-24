using UnityEngine;
[CreateAssetMenu(menuName = "Skills/Damage Skill")]
public class DamageSkillData : SkillData
{
    [Header("Damage Settings")]
    public float damageMultiplier = 1.5f; // Sát thương = 150% PHI/MAG của người chơi
    public DamageType damageType;
    public float range = 3f; // Tầm ảnh hưởng

    public override void Activate(GameObject user)
    {
        PlayerStats stats = user.GetComponent<PlayerStats>();
        if (stats == null) return;

        int baseDamage = (damageType == DamageType.Physical) ? stats.currentStats.PHI : stats.currentStats.MAG;
        int finalDamage = (int)(baseDamage * damageMultiplier);

        Debug.Log($"Skill '{skillName}' activated, dealing {finalDamage} {damageType} damage.");

        // Thêm logic tìm kẻ thù trong 'range' và gây sát thương
        // Ví dụ: OverlapCircleAll...
    }
}
using UnityEngine;
using System.Collections.Generic;

public enum ClassType
{
    Knight,
    Mage,
    Archer,
    Paladin
}
public enum DamageType { Physical, Magical }
public enum AttackType
{
    Melee, //cận chiến
    Ranged // Tầm xa 
}
[CreateAssetMenu(fileName = "NewClassDefinition", menuName = "RPG/Class Definition")]


public class ClassDefinition : ScriptableObject
{
    public List<SkillData> classSkills;
    public ClassType classType;
    public CharacterStats baseStats;
    [TextArea] public string description;
    public Sprite previewSprite;
    // Rule: class nào không cộng được chỉ số nào
    public RuntimeAnimatorController animatorOverride;
    public DamageType damageType = DamageType.Physical;
    [Header("Attack Logic")]
    public AttackType attackType = AttackType.Melee;
    public GameObject projectilePrefab; // prefabs vật bắn ra
    public float attackRange = 1f;
    public bool CanIncrease(string statName)
    {
        switch (classType)
        {
            case ClassType.Knight:
                if (statName == "MAG") return false; break;
            case ClassType.Mage:
                if (statName == "PHI") return false; break;
            case ClassType.Paladin:
                if (statName == "CRITR" || statName == "CRITD") return false; break;
            case ClassType.Archer:
                if (statName == "ARM" || statName == "MAG") return false; break;
        }
        return true;
    }
}

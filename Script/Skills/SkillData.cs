using UnityEngine;
public enum SkillType { Active, Passive}

public abstract class SkillData : ScriptableObject
{
    [Header("General info")]
    public string skillName;
    [TextArea] public string description;
    public Sprite icon;
    public SkillType skillType;
    public AnimationClip skillAnimation;

    [Header("Active Skill Stats")]
    public float cooldown = 1f;
    public int manaCost = 10;
    [Header("Leveling")]
    [Tooltip("Cấp độ tối đa của kỹ năng.")]
    public int maxLevel = 10;
    [Tooltip("Số điểm kỹ năng cần để nâng cấp.")]
    public int skillPointCost = 1;
    public abstract void Activate (GameObject user);
}

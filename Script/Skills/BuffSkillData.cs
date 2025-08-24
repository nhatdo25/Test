using UnityEngine;
public enum StatToBuff { ATK_PERCENT, DEF_PERCENT, ARMOR, MOVE_SPEED, ATTACK_SPEED, MAX_HP, MAX_MANA }

[CreateAssetMenu(menuName = "Skills/Buff Skill")]
public class BuffSkillData : SkillData
{
    [Header("Buff Settings")]
    public bool isPermanent; 
    public StatToBuff statToBuff;
    public float buffValue;
    public float duration = 10f; // Sẽ bị bỏ qua nếu isPermanent là true

    public override void Activate(GameObject user)
    {
        var buffManager = user.GetComponent<BuffManager>();
        if (buffManager != null)
        {
            buffManager.ApplyBuff(this);
        }
    }
}
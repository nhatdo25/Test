using UnityEngine;

[CreateAssetMenu(fileName = "NewStatusEffect", menuName = "RPG/Status Effect")]
public class StatusEffectData : ScriptableObject
{
    public string effectName;
    public float duration; // Thời gian tồn tại của hiệu ứng
    public float tickInterval = 1f; // Tần suất áp dụng hiệu ứng (mỗi giây)
    public EffectType effectType;

    [Header("Health over time")]
    public float healthChangePercent; // % máu thay đổi (có thể âm)

    [Header("Stat Changes")]
    public CharacterStats statModifiers; // Các chỉ số tạm thời thay đổi

    [Header("Visuals")]
    public Sprite effectIcon;
    public Color tintColor = Color.white;
}

public enum EffectType
{
    Poison, // Độc
    Stun,   // Choáng
    Slow,   // Làm chậm
    Buff    // Tăng chỉ số
}
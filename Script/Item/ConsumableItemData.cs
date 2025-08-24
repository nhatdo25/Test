using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Items/Consumable")]
public class ConsumableData : ItemData
{
    [Header("Instant Effect")]
    [Tooltip("Lượng máu hồi lại ngay lập tức.")]
    public int healthRestore;
    [Tooltip("Lượng mana hồi lại ngay lập tức.")]
    public int manaRestore;
    [Header("Status Effect")]
    public StatusEffectData statusEffectToApply;
    [Header("Effect Over Time")]
    [Tooltip("Hồi (số dương) hoặc mất (số âm) % máu tối đa mỗi giây.")]
    public float healthChangePercent;
    [Tooltip("Tổng thời gian hiệu lực (tính bằng giây).")]
    public float duration;

    private void OnValidate()
    {
        itemType = ItemType.Consumable;
        isStackable = true;
    }
}
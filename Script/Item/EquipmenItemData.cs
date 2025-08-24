// EquipmentData.cs - LỚP CON CHO TRANG BỊ
using UnityEngine;


public enum EquipmentSlot
{
    Weapon,
    Shield,
    Helmet,
    Chestplate,
    Boots,
    Accessory 
}

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Items/Equipment")]
public class EquipmentData : ItemData
{
    [Header("Equipment Info")]
    public EquipmentSlot slot;      // Vị trí trang bị
    [Header("Primary Stats")]
    public int maxHP; // Cộng máu tối đa
    public int maxMana; // Cộng mana tối đa
    [Header("Primary Combat Stats")] // Các chỉ số chính
    public int physicalArmor;       // (ARP) Giáp vật lý
    public int magicArmor;          // (ARM) Giáp phép
    public int physicalDamage;      // (PHI) Sát thương vật lý
    public int magicDamage;         // (MAG) Sát thương phép

    [Header("Secondary Combat Stats (%)")] // Các chỉ số phụ dạng %
    [Tooltip("Tỷ lệ chí mạng tính theo % (ví dụ: 15.5 tương đương 15.5%)")]
    public float critRate;          // (CRITR) Tỷ lệ chí mạng
    
    [Tooltip("Sát thương chí mạng cộng thêm tính theo % (ví dụ: 50.0 tương đương +50% sát thương khi chí mạng)")]
    public float critDamage;        // (CRITD) Sát thương chí mạng

    [Tooltip("Giảm sát thương mọi nguồn tính theo % (ví dụ: 7.0 tương đương giảm 7% sát thương)")]
    public float defensePercent;    // (DEF) Giảm sát thương theo %
    
    [Tooltip("Tăng tổng sát thương gây ra theo % (ví dụ: 10.0 tương đương tăng 10% sát thương)")]
    public float attackPercent;     // (ATK) Cộng sát thương theo %

    // Hàm này tự động gán đúng loại ItemType khi bạn tạo mới trong Editor
    private void OnValidate()
    {
        itemType = ItemType.Equipment;
        isStackable = false; // Trang bị thường không thể cộng dồn
    }
}
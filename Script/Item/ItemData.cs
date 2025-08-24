using UnityEngine;

public enum ItemType
{
    Consumable, // Vật phẩm tiêu thụ
    Equipment,  // Trang bị
    QuestItem,  // Vật phẩm nhiệm vụ
    Material    // Nguyên liệu
}

// Enum để định nghĩa độ hiếm của item.
public enum Rarity
{
    Common,     // Thường (Trắng)
    Uncommon,   // Không phổ biến (Xanh lá)
    Rare,       // Hiếm (Xanh dương)
    Epic,       // Sử thi (Tím)
    Legendary   // Huyền thoại (Cam)
}

public abstract class ItemData : ScriptableObject
{
    [Header("Information")]
    public string itemName;
    public string itemID;
    [TextArea(4, 10)]
    public string description;
    public Sprite icon;

    [Header("Classification")]
    public ItemType itemType;
    public Rarity rarity;

    [Header("Properties")]
    public bool isStackable;
    public int maxStackSize = 99;
    public int value;
}
[System.Serializable]
public class ItemDrop
{
    public ItemData item;
    [Range(0, 100)] public int dropChance; // Tỉ lệ rớt (%)
}
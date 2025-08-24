using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyDefinition", menuName = "RPG/Enemy Definition")]
public class EnemyDefinition : ScriptableObject
{
    [Header("General Stats")]
    public string enemyName;
    public EnemyType enemyType;
    public int level;
    [Header("Visuals")]
    public Sprite previewSprite;
    [Header("Combat Stats")]
    public EnemyStatsData baseStats; // Sử dụng lại CharacterStats để định nghĩa HP, PHI, MAG, v.v.
    public float attackRange = 1f;
    public float respawnTime = 10f;
    [Header("Special Abilities")]
    public StatusEffectData[] statusEffects; // Hiệu ứng xấu có thể gây ra (ví dụ: độc, choáng)
    public float effectChance = 0.2f; // Tỉ lệ gây hiệu ứng

    [Header("Loot")]
    public ItemDropTable dropTable; // Bảng tỉ lệ rớt đồ
    public int experienceGained; // Kinh nghiệm nhận được khi tiêu diệt
}

public enum EnemyType
{
    Normal,
    Elite,
    Boss
}

[CreateAssetMenu(fileName = "NewDropTable", menuName = "RPG/Item Drop Table")]
public class ItemDropTable : ScriptableObject
{
    public List<ItemDrop> drops;

    public ItemData GetRandomItem()
    {
        int totalChance = 0;
        foreach (var drop in drops)
        {
            totalChance += drop.dropChance;
        }

        int randomNumber = Random.Range(0, totalChance);
        int currentChance = 0;

        foreach (var drop in drops)
        {
            currentChance += drop.dropChance;
            if (randomNumber < currentChance)
            {
                return drop.item;
            }
        }

        return null; // Không có vật phẩm nào rớt
    }
}

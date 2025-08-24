using UnityEngine;
[System.Serializable]
public class EnemyStatsData
{
    // Các chỉ số cơ bản của kẻ địch
    public int HP;
    public int PHI; // Sát thương vật lý
    public int MAG;    // Sát thương phép thuật
    public int ARP;
    public int ARM; // Giáp
    public int ArmorPenetration; // Xuyên giáp

    // Hành vi tấn công của kẻ địch
    public float AttackSpeed; // Thời gian lặp lại sát thương
    public int DamageToPlayer;  // Sát thương gây ra cho người chơi
    // Cấu trúc để lưu các chỉ số này trong file dữ liệu
    public EnemyStatsData(int health, int physicalAttack, int magicAttack, int arp, int arm, int armorPenetration, float attackSpeed, int damageToPlayer)
    {
        HP = health;
        PHI = physicalAttack;
        MAG = magicAttack;
        ARP = arp;
        ARM = arm;
        ArmorPenetration = armorPenetration;
        AttackSpeed = attackSpeed;
        DamageToPlayer = damageToPlayer;
    }
}
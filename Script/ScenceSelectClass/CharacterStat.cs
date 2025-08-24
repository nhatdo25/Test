using UnityEngine;

[System.Serializable] // Để Unity hiển thị trong Inspector
public class CharacterStats
{
    public int maxHP;      // Máu khởi điểm 
    public int maxMana;    // Mana khởi điểm
    public int PHI;     // Sát thương vật lý
    public int MAG;     // Sát thương phép
    public int ARP;     // Giáp vật lý
    public int ARM;     // Giáp phép
    public float CRITR; // Tỷ lệ chí mạng (%)
    public float CRITD; // Sát thương chí mạng (multiplier)
    public float DEF;   // (DEF) Giảm sát thương theo %
    public float ATK;   // (ATK) Tăng sát thương theo %
    [HideInInspector] public int HP ;
    [HideInInspector] public int Mana;
    public float moveSpeed;
    public void CopyFrom(CharacterStats other)
    {
        HP = other.HP;
        Mana = other.Mana;
        PHI = other.PHI;
        MAG = other.MAG;
        ARP = other.ARP;
        ARM = other.ARM;
        CRITR = other.CRITR;
        CRITD = other.CRITD;
        DEF = other.DEF;
        ATK = other.ATK;
        maxHP = other.maxHP;
        maxMana = other.maxMana;
        moveSpeed = other.moveSpeed;
    }
}

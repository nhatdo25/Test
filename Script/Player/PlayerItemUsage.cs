using UnityEngine;

public class PlayerItemUsage : MonoBehaviour
{
    private PlayerStats playerStats;
    private PlayerStatusEffects statusEffects;

    void Start()
    {
        statusEffects = GetComponent<PlayerStatusEffects>();
        // Lấy component PlayerStats từ chính object này
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("Không tìm thấy PlayerStats trên Player!");
        }
    }

    // Đây là hàm chính để sử dụng item, sẽ được gọi từ UI hoặc Inventory
    public void UseItem(ItemData item)
    {
        if (item is ConsumableData consumable)
        {
            Debug.Log($"Sử dụng {consumable.itemName}.");

            // 1. Xử lý các hiệu ứng TỨC THỜI bằng cách gọi các hàm public
            if (consumable.healthRestore != 0)
            {
                playerStats.ChangeHealth(consumable.healthRestore);
            }
            if (consumable.manaRestore != 0)
            {
                playerStats.ChangeMana(consumable.manaRestore);
            }

            // 2. Kích hoạt các hiệu ứng THEO THỜI GIAN
            if (consumable.statusEffectToApply != null && statusEffects != null)
            {
                statusEffects.AddEffect(consumable.statusEffectToApply); // Pass the correct type
            }
            // TODO: Xóa item khỏi túi đồ
            // Inventory.instance.RemoveItem(item);
        }
        else
        {
            Debug.LogWarning($"{item.itemName} không phải là vật phẩm tiêu thụ!");
        }
    }
}
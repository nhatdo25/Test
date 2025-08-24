using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Cấu hình UI")]
    public Transform inventoryContentPanel;
    public GameObject slotPrefab;

    private List<UISlot> inventorySlots = new List<UISlot>();

    // --- THAY ĐỔI QUAN TRỌNG Ở ĐÂY ---

    // Hàm này được gọi mỗi khi GameObject chứa script này được BẬT
    private void OnEnable()
    {
        // Đăng ký lắng nghe sự kiện
        Inventory.OnInventoryChanged += DrawInventory;
        Debug.Log("--- InventoryManager đã lắng nghe tín hiệu! ---");
    }

    // Hàm này được gọi mỗi khi GameObject chứa script này bị TẮT
    private void OnDisable()
    {
        // Hủy đăng ký để tránh lỗi
        Inventory.OnInventoryChanged -= DrawInventory;
        Debug.Log("--- InventoryManager đã ngừng lắng nghe tín hiệu. ---");
    }

    void Start()
    {
        // Chỉ tạo slot và vẽ lần đầu
        GenerateSlots();
        DrawInventory();
    }

    void GenerateSlots()
    {
        foreach (Transform child in inventoryContentPanel)
        {
            Destroy(child.gameObject);
        }
        inventorySlots.Clear();

        for (int i = 0; i < Inventory.instance.inventorySpace; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, inventoryContentPanel);
            UISlot slotComponent = newSlot.GetComponent<UISlot>();
            //slotComponent.slotIndex = i;
            inventorySlots.Add(slotComponent);
        }
    }

    void DrawInventory()
    {
        Debug.Log("InventoryManager: Bắt đầu vẽ lại inventory...");
        
        if (Inventory.instance == null)
        {
            Debug.LogError("InventoryManager: Inventory.instance là null!");
            return;
        }

        List<ItemData> playerItems = Inventory.instance.items;
        Debug.Log($"InventoryManager: Có {playerItems.Count} item trong túi đồ, {inventorySlots.Count} slot UI");

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i < playerItems.Count)
            {
                // Vị trí này có item -> Update
                inventorySlots[i].UpdateSlot(playerItems[i]);
            }
            else
            {
                // Vị trí này không có item -> Clear
                inventorySlots[i].ClearSlot();
            }
        }

        Debug.Log("InventoryManager: Hoàn thành vẽ lại inventory");
    }
}
using System.Collections.Generic;
using UnityEngine;

// Một lớp nhỏ để giúp chúng ta gán các ô slot trong Inspector
[System.Serializable]
public class EquipmentSlotMapping
{
    public EquipmentSlot slotType;
    public UISlot uiSlot;
}

public class EquipmentPanelManager : MonoBehaviour
{
    [Header("Cấu hình các ô trang bị")]
    // Tạo một danh sách để bạn có thể gán từng ô trang bị
    public List<EquipmentSlotMapping> slotMappings;

    // Tạo một Dictionary để truy cập nhanh
    private Dictionary<EquipmentSlot, UISlot> equipmentSlots = new Dictionary<EquipmentSlot, UISlot>();

    void Awake()
    {
        // Chuyển danh sách từ Inspector vào Dictionary để dễ tìm kiếm
        foreach (var mapping in slotMappings)
        {
            equipmentSlots[mapping.slotType] = mapping.uiSlot;
        }
    }

    void Start()
    {
        EquipmentManager.OnEquipmentChanged += DrawEquipmentPanel;
        DrawEquipmentPanel();
    }

    private void OnDisable()
    {
        EquipmentManager.OnEquipmentChanged -= DrawEquipmentPanel;
    }

    void DrawEquipmentPanel()
    {
        if (EquipmentManager.instance == null) return;

        // Lặp qua tất cả các loại slot trong Dictionary
        foreach (var slotPair in equipmentSlots)
        {
            EquipmentSlot slotType = slotPair.Key;
            UISlot uiSlot = slotPair.Value;

            EquipmentData itemInSlot = EquipmentManager.instance.GetEquipment(slotType);

            if (itemInSlot != null)
            {
                uiSlot.UpdateSlot(itemInSlot);
            }
            else
            {
                uiSlot.ClearSlot();
            }
        }
    }
}
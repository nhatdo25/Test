using System.Collections.Generic;
using UnityEngine;
using System;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;
    public static event Action OnEquipmentChanged;
    
    private Dictionary<EquipmentSlot, EquipmentData> currentEquipment = new Dictionary<EquipmentSlot, EquipmentData>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Equip(EquipmentData newItem)
    {
        EquipmentData oldItem = null;
        currentEquipment.TryGetValue(newItem.slot, out oldItem);

        // BƯỚC 1: CẬP NHẬT DỮ LIỆU TÚI ĐỒ TRƯỚC
        // Hành động này sẽ tự phát event OnInventoryChanged để UI túi đồ vẽ lại
        Inventory.instance.RemoveItem(newItem);
        if (oldItem != null)
        {
            Inventory.instance.AddItem(oldItem);
        }

        // BƯỚC 2: CẬP NHẬT DỮ LIỆU TRANG BỊ
        currentEquipment[newItem.slot] = newItem;

        // BƯỚC 3: PHÁT TÍN HIỆU TRANG BỊ SAU CÙNG
        OnEquipmentChanged?.Invoke();
    }

    public void Unequip(EquipmentSlot slot)
    {
        if (currentEquipment.TryGetValue(slot, out EquipmentData oldItem))
        {
            // BƯỚC 1: CẬP NHẬT DỮ LIỆU TRANG BỊ TRƯỚC
            currentEquipment.Remove(slot);

            // BƯỚC 2: PHÁT TÍN HIỆU TRANG BỊ
            OnEquipmentChanged?.Invoke();

            // BƯỚC 3: CẬP NHẬT DỮ LIỆU TÚI ĐỒ SAU CÙNG
            // Hành động này sẽ tự phát event OnInventoryChanged
            Inventory.instance.AddItem(oldItem);
        }
    }

    public EquipmentData GetEquipment(EquipmentSlot slot)
    {
        currentEquipment.TryGetValue(slot, out EquipmentData item);
        return item;
    }

    public List<EquipmentData> GetCurrentEquipment()
    {
        return new List<EquipmentData>(currentEquipment.Values);
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Thêm thư viện TextMeshPro

public class ItemActionManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject actionPanel;
    public TextMeshProUGUI itemNameText;
    public Button actionButton1; // Nút đa năng 1
    public Button actionButton2; // Nút đa năng 2
    public Button cancelButton;

    private UISlot selectedSlot;

    void OnEnable()
    {
        UISlot.OnSlotClicked += OnSlotSelected;
    }
    void OnDisable()
    {
        UISlot.OnSlotClicked -= OnSlotSelected;
    }

    void Start()
    {
        cancelButton.onClick.AddListener(CancelAction); 
        actionPanel.SetActive(false);
    }

    void Update()
    {
        // Ẩn panel nếu click ra ngoài
        if (actionPanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            // (Phần này sẽ được thêm sau để phức tạp hơn, tạm thời để trống)
        }
    }
    void CancelAction()
    {
        // Đơn giản là ẩn panel đi
        actionPanel.SetActive(false);
        Debug.Log("Hủy hành động.");
    }
    void OnSlotSelected(UISlot slot)
    {
        selectedSlot = slot;
        ItemData item = slot.GetCurrentItem();
        if (item == null) return;

        // Cập nhật tên và vị trí panel
        itemNameText.text = item.itemName;
        actionPanel.transform.position = slot.transform.position + new Vector3(120, 0, 0);
        actionPanel.SetActive(true);

        // Xóa các listener cũ để tránh lỗi
        actionButton1.onClick.RemoveAllListeners();
        actionButton2.onClick.RemoveAllListeners();

        // Cấu hình các nút bấm tùy theo loại slot
        if (slot.slotType == SlotType.INVENTORY)
        {
            // Nút 1: Sử dụng / Trang bị
            actionButton1.GetComponentInChildren<TextMeshProUGUI>().text = "Sử dụng";
            if (item is EquipmentData)
            {
                actionButton1.GetComponentInChildren<TextMeshProUGUI>().text = "Trang bị";
                actionButton1.onClick.AddListener(EquipItem);
            }
            else if (item is ConsumableData)
            {
                actionButton1.onClick.AddListener(UseItem);
            }
            actionButton1.gameObject.SetActive(true);

            // Nút 2: Vứt bỏ
            actionButton2.GetComponentInChildren<TextMeshProUGUI>().text = "Vứt bỏ";
            actionButton2.onClick.AddListener(DropItem);
            actionButton2.gameObject.SetActive(true);
        }
        else if (slot.slotType == SlotType.EQUIPMENT)
        {
            // Nút 1: Cất vào túi
            actionButton1.GetComponentInChildren<TextMeshProUGUI>().text = "Cất vào túi";
            actionButton1.onClick.AddListener(UnequipItem);
            actionButton1.gameObject.SetActive(true);

            // Nút 2: Vứt bỏ
            actionButton2.GetComponentInChildren<TextMeshProUGUI>().text = "Vứt bỏ";
            actionButton2.onClick.AddListener(DropItem); // Có thể tạo hàm DropEquippedItem riêng nếu logic khác
            actionButton2.gameObject.SetActive(true);
        }
    }

    void EquipItem()
    {
        if (selectedSlot.GetCurrentItem() is EquipmentData equipment)
        {
            EquipmentManager.instance.Equip(equipment);
        }
        actionPanel.SetActive(false);
    }

    void UnequipItem()
    {
        if (selectedSlot.GetCurrentItem() is EquipmentData equipment)
        {
            EquipmentManager.instance.Unequip(selectedSlot.equipmentSlotType);
        }
        actionPanel.SetActive(false);
    }

    void UseItem()
    {
        if (selectedSlot.GetCurrentItem() is ConsumableData consumable)
        {
            Debug.Log($"Sử dụng {consumable.itemName}");
            // TODO: Logic sử dụng vật phẩm
            Inventory.instance.RemoveItem(consumable);
        }
        actionPanel.SetActive(false);
    }

    void DropItem()
    {
        if (selectedSlot != null && selectedSlot.GetCurrentItem() != null)
        {
            ItemData itemToDrop = selectedSlot.GetCurrentItem();
            // Nếu là trang bị đang mặc -> cởi ra trước
            if (selectedSlot.slotType == SlotType.EQUIPMENT)
            {
                EquipmentManager.instance.Unequip(selectedSlot.equipmentSlotType);
            }
            // Xóa khỏi túi đồ
            Inventory.instance.RemoveItem(itemToDrop);
            Debug.Log($"Vứt bỏ {itemToDrop.itemName}");
        }
        actionPanel.SetActive(false);
    }
}
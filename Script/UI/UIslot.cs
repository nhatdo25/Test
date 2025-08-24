using UnityEngine;
using UnityEngine.UI;
public enum SlotType
{
    INVENTORY,
    EQUIPMENT
}
public class UISlot : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image itemIcon;
    private ItemData currentItem;
    [Header("Slot Configuration")]
    public SlotType slotType;
    public EquipmentSlot equipmentSlotType;
    public int slotIndex;
    private Button button;
    public static event System.Action<UISlot> OnSlotClicked;
    public Sprite emptySlotSprite;
    void Awake()
    {
        // Lấy component Button và gán sự kiện click
        button = GetComponent<Button>();
        if (button == null)
        {
            // Tự động thêm Button nếu chưa có
            button = gameObject.AddComponent<Button>();
        }
        button.onClick.AddListener(OnButtonClicked);
        
        // Gán tham chiếu cho icon nếu chưa có
        if (itemIcon == null)
        {
            // Giả định icon là đối tượng con đầu tiên có component Image
            itemIcon = transform.GetChild(0).GetComponent<Image>();
        }

        ClearSlot();
    }

    // Xóa vật phẩm khỏi ô này
    public void ClearSlot()
    {
        currentItem = null;
        if (itemIcon != null)
        {
            itemIcon.sprite = emptySlotSprite;
            itemIcon.gameObject.SetActive(false);
        }
        button.interactable = false; // Ô trống thì không bấm được
    }

    // Đặt vật phẩm vào ô này và cập nhật hình ảnh
    public void UpdateSlot(ItemData item)
    {
        currentItem = item;
        if (itemIcon != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.gameObject.SetActive(true);
        }
        button.interactable = true; // Ô có đồ thì bấm được
    }

    // Khi người chơi click vào slot
    void OnButtonClicked()
    {
        if (currentItem != null)
        {
            // Phát tín hiệu ra ngoài, gửi kèm thông tin của chính slot này
            OnSlotClicked?.Invoke(this);
        }
    }

    // Hàm để các script khác có thể lấy thông tin item
    public ItemData GetCurrentItem() => currentItem;
}
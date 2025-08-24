using System.Collections.Generic;
using UnityEngine;
using System; // Cần thiết để sử dụng Action

public class Inventory : MonoBehaviour
{
    // Singleton pattern để dễ dàng truy cập từ mọi nơi
    public static Inventory instance;

    // Event sẽ được phát đi mỗi khi túi đồ có thay đổi
    public static event Action OnInventoryChanged;

    public List<ItemData> items = new List<ItemData>();
    public int inventorySpace = 20; // Ví dụ: giới hạn 20 ô

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Giữ lại khi chuyển scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AddItem(ItemData item)
    {
        // Kiểm tra item có hợp lệ không
        if (item == null)
        {
            Debug.LogError("AddItem: Không thể thêm item null!");
            return false;
        }

        // Kiểm tra xem còn chỗ trống không
        if (items.Count >= inventorySpace)
        {
            Debug.LogWarning($"AddItem: Túi đồ đã đầy! Không thể thêm {item.itemName}");
            return false;
        }

        // Kiểm tra xem item đã có trong túi chưa (để tránh duplicate)
        if (items.Contains(item))
        {
            Debug.LogWarning($"AddItem: Item {item.itemName} đã có trong túi đồ! Bỏ qua thêm duplicate");
            return false;
        }

        // Thêm item vào túi đồ
        items.Add(item);
        Debug.Log($"AddItem: Đã thêm {item.itemName} vào túi đồ. Tổng số item: {items.Count}/{inventorySpace}");

        // Phát tín hiệu "Túi đồ đã thay đổi!"
        OnInventoryChanged?.Invoke();

        return true;
    }

    public void RemoveItem(ItemData item)
    {
        // Kiểm tra item có hợp lệ không
        if (item == null)
        {
            Debug.LogError("RemoveItem: Không thể xóa item null!");
            return;
        }

        // Kiểm tra xem item có trong túi không
        if (!items.Contains(item))
        {
            Debug.LogWarning($"RemoveItem: Item {item.itemName} không có trong túi đồ!");
            return;
        }

        if (items.Remove(item))
        {
            Debug.Log($"RemoveItem: Đã xóa {item.itemName} khỏi túi đồ. Tổng số item: {items.Count}/{inventorySpace}");
            OnInventoryChanged?.Invoke();
            // KHÔNG gọi OnInventoryChanged ngay lập tức để tránh race condition
            // UI sẽ được cập nhật sau khi EquipmentManager hoàn tất
            Debug.Log($"RemoveItem: Không gọi OnInventoryChanged để tránh race condition");
        }
        else
        {
            Debug.LogError($"RemoveItem: Lỗi khi xóa {item.itemName} khỏi túi đồ!");
        }
    }
    public void SwapItems(ItemData itemA, ItemData itemB)
    {
        // Tìm vị trí index của 2 item trong danh sách
        int indexA = items.IndexOf(itemA);
        int indexB = items.IndexOf(itemB);

        // Nếu cả 2 item đều tồn tại trong túi đồ, hoán đổi chúng
        if (indexA != -1 && indexB != -1)
        {
            items[indexA] = itemB;
            items[indexB] = itemA;

            Debug.Log($"Hoán đổi {itemA.itemName} và {itemB.itemName}");
            OnInventoryChanged?.Invoke(); // Phát tín hiệu để UI vẽ lại
        }
    }
    // Thêm item nhưng không phát tín hiệu

}
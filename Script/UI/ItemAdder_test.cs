using UnityEngine;

public class ItemAdder_Test : MonoBehaviour
{
    [Tooltip("Kéo một ItemData asset từ Project vào đây.")]
    public ItemData itemToAdd;

    [Tooltip("Nhấn phím này để thêm item vào túi đồ.")]
    public KeyCode addKey = KeyCode.T;

    void Update()
    {
        // DÒNG TEST QUAN TRỌNG NHẤT
       // Debug.Log("ItemAdder_Test Update is running frame by frame...");

        // Nếu phím được nhấn
        if (Input.GetKeyDown(addKey))
        {
            if (itemToAdd != null)
            {
                Inventory.instance.AddItem(itemToAdd);
            }
            else
            {
                Debug.LogWarning("Chưa chọn vật phẩm để thêm!");
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Tìm PlayerStats và gọi hàm cộng 10 điểm
                player.GetComponent<PlayerStats>()?.AddStatPoints(10);
            }
        }
    }
}
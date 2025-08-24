using UnityEngine;
using UnityEngine.UI; 
public class CustomCursor : MonoBehaviour
{
    // Kéo thả đối tượng UI Image của con trỏ vào đây trong Inspector
    public RectTransform cursorImage;

    void Start()
    {
        // Ẩn con trỏ mặc định của hệ điều hành khi game bắt đầu
        Cursor.visible = false;
    }

    void Update()
    {
        // Di chuyển vị trí của UI Image theo vị trí của con trỏ chuột trên màn hình
        // Input.mousePosition trả về tọa độ pixel trên màn hình, rất phù hợp với UI Canvas
        cursorImage.position = Input.mousePosition;
    }

    // (Tùy chọn) Đảm bảo con trỏ hiện lại khi game bị tắt hoặc mất focus
    private void OnDisable()
    {
        Cursor.visible = true;
    }
}
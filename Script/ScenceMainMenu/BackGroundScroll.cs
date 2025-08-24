using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed = 0.1f; // tốc độ cuộn
    private RawImage image;

    void Start()
    {
        image = GetComponent<RawImage>();
    }

    void Update()
    {
        if (image != null)
        {
            // Tính toán offset mới cho UV (chỉ thay đổi trục X)
            float xOffset = Time.time * scrollSpeed;

            // Áp dụng offset vào uvRect của RawImage
            // uvRect.x, uvRect.y, uvRect.width, uvRect.height
            // Chúng ta chỉ thay đổi uvRect.x để cuộn ngang
            image.uvRect = new Rect(xOffset, 0f, 1f, 1f);
        }
    }
}

using System.Collections;
using UnityEngine;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    [Header("Level Up Notification")]
    [Tooltip("Kéo đối tượng Text 'LevelUp_Text' vào đây.")]
    public TextMeshProUGUI levelUpText;
    [Tooltip("Thời gian hiển thị (tính bằng giây).")]
    public float displayDuration = 2.5f;

    void OnEnable()
    {
        // Lắng nghe sự kiện lên cấp từ PlayerStats
        PlayerStats.OnPlayerLevelUp += ShowLevelUpNotification;
    }

    void OnDisable()
    {
        PlayerStats.OnPlayerLevelUp -= ShowLevelUpNotification;
    }

    void Start()
    {
        // Đảm bảo text đã tắt lúc ban đầu
        if (levelUpText != null)
        {
            levelUpText.gameObject.SetActive(false);
        }
    }

    // Hàm này được gọi mỗi khi PlayerStats phát tín hiệu lên cấp
    private void ShowLevelUpNotification(int newLevel)
    {
        if (levelUpText != null)
        {
            // Dừng các coroutine cũ (nếu có) và bắt đầu một coroutine mới
            StopAllCoroutines();
            StartCoroutine(LevelUpRoutine());
        }
    }

    // Coroutine để xử lý việc hiện ra rồi mờ đi
    private IEnumerator LevelUpRoutine()
    {
        // Bật text lên
        levelUpText.gameObject.SetActive(true);

        // Chờ trong một khoảng thời gian
        yield return new WaitForSeconds(displayDuration);

        // Tắt text đi
        levelUpText.gameObject.SetActive(false);
    }
}
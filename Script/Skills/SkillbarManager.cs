using System.Collections.Generic;
using UnityEngine;

public class SkillbarManager : MonoBehaviour
{
    [Header("Skill Slots")]
    [Tooltip("Kéo 5 ô skill trên HUD vào đây theo đúng thứ tự")]
    public List<GameObject> skillSlots;

    void OnEnable()
    {
        // Lắng nghe sự kiện lên cấp từ PlayerStats
        PlayerStats.OnPlayerLevelUp += OnPlayerLeveledUp;
    }

    void OnDisable()
    {
        PlayerStats.OnPlayerLevelUp -= OnPlayerLeveledUp;
    }

    void Start()
    {
        // Tìm PlayerStats và cập nhật lại thanh skill bar lần đầu
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            OnPlayerLeveledUp(playerStats.level);
        }
    }

    // Hàm này được gọi mỗi khi người chơi lên cấp
    private void OnPlayerLeveledUp(int newLevel)
    {
        // Ban đầu có 2 ô
        skillSlots[0].SetActive(true);
        skillSlots[1].SetActive(true);

        // Mở khóa ô thứ 3 ở cấp 10
        skillSlots[2].SetActive(newLevel >= 10);

        // Mở khóa ô thứ 4 ở cấp 20
        skillSlots[3].SetActive(newLevel >= 20);

        // Mở khóa ô thứ 5 ở cấp 30
        // Dựa theo yêu cầu "thêm 2 ô", nhưng vì đã có 4 ô nên chỉ còn 1 ô thứ 5
        skillSlots[4].SetActive(newLevel >= 30);
    }
}
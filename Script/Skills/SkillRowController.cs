using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillRowController : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillLevelText;
    public Button levelUpButton;

    private SkillData skillData;
    private SkillTreeUIManager uiManager;

    // Hàm này được gọi từ Manager chính để thiết lập
    public void Initialize(SkillData skill, int currentLevel, SkillTreeUIManager manager)
    {
        skillData = skill;
        uiManager = manager;

        // Gán hình ảnh và tên ban đầu
        iconImage.sprite = skill.icon;
        skillNameText.text = skill.skillName;

        // Gán sự kiện cho nút '+' và nút chọn cả hàng
        levelUpButton.onClick.AddListener(OnLevelUpClicked);
        GetComponent<Button>().onClick.AddListener(OnRowSelected);

        // Cập nhật hiển thị cấp độ
        UpdateLevel(currentLevel);
    }

    // Khi nút '+' được nhấn
    private void OnLevelUpClicked()
    {
        // Thông báo cho Manager chính rằng nút này đã được nhấn
        uiManager.OnLevelUpClicked(skillData, this);
    }

    // Khi cả hàng được nhấn
    private void OnRowSelected()
    {
        // Thông báo cho Manager chính để hiển thị chi tiết
        uiManager.OnSkillRowSelected(skillData);
    }

    // Hàm công khai để Manager có thể cập nhật lại cấp độ
    public void UpdateLevel(int newLevel)
    {
        skillLevelText.text = $"Cấp {newLevel} / {skillData.maxLevel}";
    }
}
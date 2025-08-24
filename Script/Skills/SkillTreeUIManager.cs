using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SkillTreeUIManager : MonoBehaviour
{
    [Header("Core References")]
    public SkillManager skillManager;
    public PlayerStats playerStats;
    public TextMeshProUGUI skillPointText;
    public Transform skillListParent;
    public GameObject skillRowPrefab;

    [Header("Skill Detail Panel")]
    public GameObject detailPanel;
    public Image detailIcon; // Thêm tham chiếu cho Icon
    public TextMeshProUGUI detailNameText;
    public TextMeshProUGUI detailDescriptionText;

    private List<SkillRowController> skillRows = new List<SkillRowController>();
    private bool isInitialized = false;

    // Hàm này được gọi mỗi khi GameObject chứa script này được BẬT
    void OnEnable()
    {
        // Mỗi khi panel được bật, hãy cập nhật lại điểm kỹ năng
        if (isInitialized)
        {
            UpdateSkillPointText(playerStats.skillPoints);
        }
    }

    // Hàm này sẽ được gọi từ GameSceneManager
    public void Initialize()
    {
        if (isInitialized || skillManager == null || playerStats == null) return;

        PlayerStats.OnSkillPointsChanged += UpdateSkillPointText;
        CreateSkillRows(); // Bây giờ sẽ tạo đúng skill
        UpdateSkillPointText(playerStats.skillPoints);
        detailPanel.SetActive(false);
        isInitialized = true;
    }

    private void OnDisable()
    {
        PlayerStats.OnSkillPointsChanged -= UpdateSkillPointText;
    }

    void CreateSkillRows()
    {
        // Xóa các hàng cũ để tránh nhân đôi khi khởi tạo lại
        foreach (Transform child in skillListParent)
        {
            Destroy(child.gameObject);
        }
        skillRows.Clear();

        // THAY ĐỔI Ở ĐÂY: Duyệt qua danh sách skill của class hiện tại
        foreach (SkillData skill in skillManager.CurrentClassSkills)
        {
            GameObject rowObj = Instantiate(skillRowPrefab, skillListParent);
            SkillRowController rowController = rowObj.GetComponent<SkillRowController>();

            int currentLevel = skillManager.GetSkillLevel(skill);
            rowController.Initialize(skill, currentLevel, this);
            skillRows.Add(rowController);
        }
    }

    public void OnSkillRowSelected(SkillData skill)
    {
        detailPanel.SetActive(true);
        detailIcon.sprite = skill.icon; // Cập nhật Icon
        detailNameText.text = skill.skillName;
        detailDescriptionText.text = skill.description;
    }

    public void OnLevelUpClicked(SkillData skill, SkillRowController row)
    {
        if (skillManager.LevelUpSkill(skill))
        {
            int newLevel = skillManager.GetSkillLevel(skill);
            row.UpdateLevel(newLevel);
        }
    }

    void UpdateSkillPointText(int amount)
    {
        skillPointText.text = $"Điểm Kỹ Năng: {amount}";
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class StatRowUI
{
    public string statName;
    public TextMeshProUGUI valueText;
    public Button increaseButton;
    [HideInInspector] public int pointsAdded;
}

public class StatAllocationUI : MonoBehaviour
{
    [Header("Core References")]
    public GameObject statPanel;
    public TextMeshProUGUI pointsText;
    public Button confirmButton;
    public Button resetButton;
    private PlayerStats playerStats;
    private ClassDefinition currentClass;
    //public KeyCode toggleKey = KeyCode.I;
    [Header("Container")]
    [Tooltip("Kéo GameObject StatList vào đây")]
    public Transform statListContainer; 
    
    private List<StatRowController> rowControllers;
    private int pointsToSpend;

    
    void Start()
    {
        confirmButton.onClick.AddListener(ConfirmChanges);
        resetButton.onClick.AddListener(ResetChanges);
        statPanel.SetActive(false);
    }

    

    public void Initialize(PlayerStats stats, ClassDefinition playerClass)
    {
        Debug.Log("StatAllocationUI Initialize: ĐÃ ĐƯỢC GỌI! Bắt đầu gán class và cập nhật UI.");
        playerStats = stats;
        currentClass = playerClass;
        rowControllers = statListContainer.GetComponentsInChildren<StatRowController>(true).ToList();
        Debug.Log($"StatAllocationUI Initialize: Đã tìm thấy {rowControllers.Count} StatRowController bên trong StatList.");

        foreach (var controller in rowControllers)
        {
            controller.Initialize(this, controller.statName);
        }
        UpdateAllUI();
    }
    void OnEnable()
    {
        PlayerStats.OnStatPointsChanged += UpdatePointsToSpend;
    }

    void OnDisable()
    {
        PlayerStats.OnStatPointsChanged -= UpdatePointsToSpend;
    }
    // Hàm này sẽ tự động được gọi mỗi khi PlayerStats phát tín hiệu
    private void UpdatePointsToSpend(int newPointTotal)
    {
        // Cập nhật lại số điểm có sẵn trong UI
        pointsToSpend = newPointTotal;
        // Vẽ lại toàn bộ UI để hiển thị số điểm mới và cập nhật các nút
        UpdateAllUI();
    }
    public void OpenStatPanel()
    {
        if (playerStats == null || currentClass == null) return;
        //ResetChanges();
        UpdateAllUI();
        statPanel.SetActive(true);
    }

    public void RequestAddPoint(StatRowController row)
    {
        if (pointsToSpend > 0)
        {
            pointsToSpend--;
            row.AddPoint();
            UpdateAllUI();
        }
    }
    void ResetChanges()
{
    // Ra lệnh cho PlayerStats tự reset
    playerStats.ResetStats();
    
    // Cập nhật lại các biến tạm và giao diện
    pointsToSpend = playerStats.availableStatPoints;
    foreach (var controller in rowControllers)
    {
        controller.ResetPoints();
    }
    UpdateAllUI();
}

    void ConfirmChanges()
    {
        CharacterStats pointsToAdd = new CharacterStats();
        foreach (var controller in rowControllers)
        {
            switch (controller.statName)
            {
                case "HP": pointsToAdd.maxHP = controller.GetPointsAdded(); break;
                case "Mana": pointsToAdd.maxMana = controller.GetPointsAdded(); break;
                case "PHI": pointsToAdd.PHI = controller.GetPointsAdded(); break;
                case "MAG": pointsToAdd.MAG = controller.GetPointsAdded(); break;
                case "ARP": pointsToAdd.ARP = controller.GetPointsAdded(); break;
                case "ARM": pointsToAdd.ARM = controller.GetPointsAdded(); break;
                case "CRITR": pointsToAdd.CRITR  = controller.GetPointsAdded(); break;
                case "CRITD": pointsToAdd.CRITD = controller.GetPointsAdded(); break;
            //  case "MoveSpeed": pointsToAdd.moveSpeed = controller.GetPointsAdded(); break;
            }
        }
        playerStats.ApplyStatAllocation(pointsToAdd);
        statPanel.SetActive(false);
    }

    void UpdateAllUI()
    {
        if (playerStats == null || currentClass == null) return;

        pointsText.text = $"Điểm còn lại: {pointsToSpend}";
        //Lấy chỉ số hiện tại
        CharacterStats currentDisplayStats = playerStats.GetCurrentStats();
        //Cập nhật UI
        foreach (var row in rowControllers)
        {
            float baseValue = 0;
            switch (row.statName)
            {
                case "HP": baseValue = currentDisplayStats.maxHP; break;
                case "Mana": baseValue = currentDisplayStats.maxMana; break;
                case "PHI": baseValue = currentDisplayStats.PHI; break;
                case "MAG": baseValue = currentDisplayStats.MAG; break;
                case "ARP": baseValue = currentDisplayStats.ARP; break;
                case "ARM": baseValue = currentDisplayStats.ARM; break;
                case "CRITR": baseValue = currentDisplayStats.CRITR; break;
                case "CRITD": baseValue = currentDisplayStats.CRITD; break;
            //  case "MoveSpeed": baseValue = currentDisplayStats.moveSpeed; break;
            }
            
            // Tính toán giá trị để hiển thị
            float displayValue = baseValue + row.GetPointsAdded();

            // Gọi hàm public để cập nhật text
            string displayText = displayValue.ToString();
            if (row.statName == "CRITR" || row.statName == "CRITD")
            {
                displayText = displayValue.ToString("F1") + "%";
            }
            row.UpdateDisplayText(displayText);

            // Áp dụng luật từ ClassDefinition và gọi hàm public để cập nhật nút
            bool canIncrease = currentClass.CanIncrease(row.statName);
            row.UpdateButtonInteractable(pointsToSpend > 0 && canIncrease);
        }
        confirmButton.interactable = (pointsToSpend < playerStats.availableStatPoints);
    }
}
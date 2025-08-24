using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatRowController : MonoBehaviour
{
    [Header("Stat Info")]
    public string statName; // Sẽ được đặt trong Inspector (VD: "maxHP", "PHI")

    [Header("UI References (Assign in Prefab)")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI valueText;
    public Button increaseButton;

    private int pointsAdded = 0;
    private StatAllocationUI mainController;

    // Được gọi từ Manager chính để "giới thiệu"
    public void Initialize(StatAllocationUI controller, string displayName)
    {
        mainController = controller;
        increaseButton.onClick.AddListener(OnIncreaseClicked);
         nameText.text = displayName;
        Debug.Log($"StatRowController '{statName}': Đã được Initialize.");
    }

    // Khi nút '+' được nhấn, thông báo cho Manager chính
    private void OnIncreaseClicked()
    {
        if (mainController != null)
        {
            mainController.RequestAddPoint(this);
        }
    }

    public void AddPoint()
    {
        pointsAdded++;
    }

    public int GetPointsAdded()
    {
        return pointsAdded;
    }

    public void ResetPoints()
    {
        pointsAdded = 0;
    }

    public void UpdateDisplayText(string text)
    {
        valueText.text = text;
         Debug.Log($"StatRowController '{statName}': Cập nhật value text thành '{text}'.");
    }

    public void UpdateButtonInteractable(bool isInteractable)
    {
        increaseButton.interactable = isInteractable;
    }
}
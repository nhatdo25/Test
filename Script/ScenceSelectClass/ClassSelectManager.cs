using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ClassSelectManager : MonoBehaviour
{
    [Header("UI")]
    public Image previewImage;   // nơi hiển thị ảnh nhân vật
    public TextMeshProUGUI infoText;        // nơi hiển thị mô tả

    [Header("Class Data")]
    public ClassDefinition[] classOptions; // drag 4 asset vào đây

    private int selectedIndex = -1;

    public void SelectClass(int index)
    {
        selectedIndex = index;
        ClassDefinition def = classOptions[index];

        // Hiện sprite
        if (previewImage != null)
            previewImage.sprite = def.previewSprite;

        // Hiện mô tả + stats
        var s = def.baseStats;
        infoText.text =
            $"{def.classType}\n{def.description}\n\n" +
            $"HP: {s.HP}\nMana: {s.Mana}\n" +
            $"PHI: {s.PHI}\nMAG: {s.MAG}\n" +
            $"ARP: {s.ARP}\nARM: {s.ARM}\n" +
            $"CRITR: {s.CRITR}\nCRITD: {s.CRITD}";
    }

    public void ConfirmSelection()
    {
        if (selectedIndex == -1) return;

        // Lấy class đã chọn
        //ClassDefinition selectedClass = classOptions[selectedIndex];
        string className = classOptions[selectedIndex].classType.ToString();
        Debug.Log($"[BƯ-ỚC 1 - LỰA CHỌN] Người chơi đã chọn: {className}. Bắt đầu lưu vào GameManager.");
        // ---------------------------------

        // Lưu vào GameManager
        GameManager.Instance.SetPlayerClass(className);

        // Chuyển sang GameScene
        SceneManager.LoadScene("Village");
    }
}

using UnityEngine;

public class UIToggleController : MonoBehaviour
{
    [Header("Inventory & Equipment")]
    public GameObject equipmentPanel;
    public KeyCode equipmentToggleKey = KeyCode.Tab;

    [Header("Stat Allocation")]
    public GameObject statPanel;
    public KeyCode statToggleKey = KeyCode.I;

    // --- SKILL TREE ---
    [Header("Skill Tree")]
    public GameObject skillTreePanel;
    public KeyCode skillTreeToggleKey = KeyCode.K;

    private StatAllocationUI statAllocationScript;
    

    void Start()
    {
        if (statPanel != null)
        {
            statAllocationScript = statPanel.GetComponent<StatAllocationUI>();
        }

        // Ẩn tất cả các panel khi bắt đầu
        equipmentPanel?.SetActive(false);
        statPanel?.SetActive(false);
        skillTreePanel?.SetActive(false); // <-- Thêm dòng này
    }

    void Update()
    {
        // Lắng nghe phím cho panel trang bị (Tab)
        if (Input.GetKeyDown(equipmentToggleKey))
        {
            TogglePanel(equipmentPanel);
        }

        // Lắng nghe phím cho panel cộng điểm (I)
        if (Input.GetKeyDown(statToggleKey))
        {
            if (statPanel != null && !statPanel.activeSelf)
            {
                statAllocationScript?.OpenStatPanel();
            }
            else
            {
                TogglePanel(statPanel);
            }
        }

        // --- THÊM MỚI: LẮNG NGHE PHÍM CHO SKILL TREE (K) ---
        if (Input.GetKeyDown(skillTreeToggleKey))
        {
            TogglePanel(skillTreePanel);
        }
    }

    // Hàm chung để bật/tắt một panel bất kỳ
    void TogglePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
}
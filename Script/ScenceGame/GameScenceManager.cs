using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [Header("Player Prefabs")]
    public GameObject knightPrefab;
    public GameObject magePrefab;
    public GameObject archerPrefab;
    public GameObject paladinPrefab;

    [Header("Scene References")]
    public Transform spawnPoint;
    public StatAllocationUI statAllocationUI;
    public SkillTreeUIManager skillUIManager;

    private void Start()
    {
        SpawnAndInitializePlayer();
    }

    private void SpawnAndInitializePlayer()
    {
        // 1. Lấy thông tin Class đã chọn
        ClassDefinition classDef = GameManager.Instance.playerClass;
        if (classDef == null)
        {
            Debug.LogError("LỖI: Không có class nào được chọn từ GameManager!");
            return;
        }

        // 2. Chọn Prefab tương ứng
        GameObject prefabToSpawn = GetPrefabForClass(classDef.classType);
        if (prefabToSpawn == null)
        {
            Debug.LogError($"LỖI: Không tìm thấy Prefab cho class {classDef.classType}!");
            return;
        }

        // 3. Tạo đối tượng Player
        GameObject player = Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
        player.tag = "Player";

        // 4. Lấy TẤT CẢ các component cần thiết từ Player
        PlayerStats stats = player.GetComponent<PlayerStats>();
        PlayerController controller = player.GetComponent<PlayerController>();
        SkillManager skillManager = player.GetComponent<SkillManager>();

        // --- BƯỚC KHỞI TẠO THEO ĐÚNG THỨ TỰ ---

        // 5. KHỞI TẠO CÁC COMPONENT DỮ LIỆU TRƯỚC
        if (stats != null) stats.InitializeStats(classDef.baseStats);
        if (skillManager != null) skillManager.Initialize(classDef);

        // 6. KHỞI TẠO CÁC COMPONENT PHỤ THUỘC VÀO DỮ LIỆU SAU
        if (controller != null) controller.Initialize(classDef);

        // 7. CUỐI CÙNG, KẾT NỐI PLAYER VỚI CÁC HỆ THỐNG TRONG SCENE
        ConnectPlayerToSceneSystems(player, stats, skillManager, classDef);
    }

    private GameObject GetPrefabForClass(ClassType classType)
    {
        switch (classType)
        {
            case ClassType.Knight: return knightPrefab;
            case ClassType.Mage: return magePrefab;
            case ClassType.Archer: return archerPrefab;
            case ClassType.Paladin: return paladinPrefab;
            default: return null;
        }
    }

    private void ConnectPlayerToSceneSystems(GameObject player, PlayerStats stats, SkillManager skillManager, ClassDefinition classDef)
    {
        // Kết nối Camera
        CameraFollow cameraFollow = Camera.main?.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.target = player.transform;
        }

        // "Giới thiệu" Player cho các UI Manager
        if (statAllocationUI != null)
        {
            statAllocationUI.Initialize(stats, classDef);
        }

        if (skillUIManager != null)
        {
            skillUIManager.skillManager = skillManager;
            skillUIManager.playerStats = stats;
            skillUIManager.Initialize(); // Ra lệnh cho UI tự khởi tạo và vẽ
            Debug.Log("Đã kết nối và khởi tạo SkillUIManager.");
        }
    }
}
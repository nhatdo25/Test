using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Data Source")]
    public EnemyDefinition enemyData;

    [Header("UI References")]
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI enemyLevelText;
    public Slider healthBarSlider;

    // --- Internal State ---
    private EnemyStatsData currentStats;
    private int currentHP;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private float lastAttackTime = -999f;
    private Vector3 startingPosition;
    private Coroutine attackCoroutine;
    public EnemySpawner spawner;
    //
    //[Header("Loot")]
    //public int experienceValue ;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingPosition = transform.position;
        Initialize(enemyData);
    }

    void Start()
    {
        // Tìm người chơi một lần khi bắt đầu
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        if (player == null || enemyData == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Kiểm tra xem người chơi có trong tầm đánh không
        if (distanceToPlayer <= enemyData.attackRange)
        {
            // Nếu đủ gần, bắt đầu tấn công
            TryToAttack();
        }
        else
        {
            // Nếu người chơi rời khỏi tầm, ngừng tấn công
            StopAttacking();
        }
    }

    public void Initialize(EnemyDefinition data)
    {
        enemyData = data;
        currentStats = new EnemyStatsData(
            data.baseStats.HP, data.baseStats.PHI, data.baseStats.MAG,
            data.baseStats.ARP, data.baseStats.ARM, data.baseStats.ArmorPenetration,
            data.baseStats.AttackSpeed, data.baseStats.DamageToPlayer
        );

        currentHP = currentStats.HP;
        spriteRenderer.sprite = enemyData.previewSprite;

        // Cập nhật UI
        if (enemyNameText != null) enemyNameText.text = enemyData.enemyName;
        if (enemyLevelText != null) enemyLevelText.text = "Lv. " + enemyData.level.ToString();
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = currentStats.HP;
            healthBarSlider.value = currentHP;
        }
    }

    public void TakeDamage(int damage)
    {
        // Giảm sát thương dựa trên giáp (ví dụ đơn giản)
        int finalDamage = Mathf.Max(damage - currentStats.ARP, 1);
        currentHP -= finalDamage;
        currentHP = Mathf.Max(currentHP, 0); // Đảm bảo máu không âm

        Debug.Log($"{enemyData.enemyName} nhận {finalDamage} sát thương. Còn lại {currentHP} HP.");
        UpdateHealthBar();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void TryToAttack()
    {
        // Chỉ bắt đầu coroutine tấn công nếu nó chưa chạy
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(AttackRoutine());
        }
    }

    private void StopAttacking()
    {
        // Dừng coroutine tấn công nếu nó đang chạy
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (true) // Vòng lặp tấn công vô hạn (sẽ bị dừng bởi StopAttacking)
        {
            Debug.Log($"{enemyData.enemyName} tấn công người chơi!");

            // Lấy component PlayerStats của người chơi và gây sát thương
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(currentStats.DamageToPlayer);
                TryApplyStatusEffect(playerStats);
            }

            // Chờ theo AttackSpeed
            yield return new WaitForSeconds(currentStats.AttackSpeed);
        }
    }

    private void Die()
    {
        Debug.Log($"{enemyData.enemyName} đã bị tiêu diệt!");
        StopAllCoroutines();

        // Tìm đối tượng Player trong scene bằng Tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            // Lấy component PlayerStats và gọi hàm cộng EXP
            playerObject.GetComponent<PlayerStats>()?.AddExperience(enemyData.experienceGained);
        }
        // GameManager.Instance.AddExperience(enemyData.experienceGained);
        // enemyData.dropTable?.DropItem(transform.position);

        // Thay vì tự hồi sinh, hãy thông báo cho spawner
        if (spawner != null)
        {
            spawner.RespawnEnemy(gameObject);
        }
        else
        {
            // Nếu không có spawner, thì chỉ phá hủy
            Destroy(gameObject);
        }
    }

    private void Respawn()
    {
        Debug.Log($"{enemyData.enemyName} đã hồi sinh!");
        transform.position = startingPosition;
        Initialize(enemyData); // Khởi tạo lại toàn bộ chỉ số
        gameObject.SetActive(true);
    }

    private void TryApplyStatusEffect(PlayerStats player)
    {
        // ... code gây hiệu ứng ...
    }

    private void UpdateHealthBar()
    {
        if (healthBarSlider != null) healthBarSlider.value = currentHP;
    }
}
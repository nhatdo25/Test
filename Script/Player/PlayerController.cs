using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Core")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private PlayerStats playerStats;
    public ClassDefinition currentClass;

    [Header("Combat References")]
    public Transform attackPoint;
    public Transform firePoint;
    public LayerMask enemyLayers;
    private SkillManager skillManager;
    private bool isAttacking = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>();
        skillManager = GetComponent<SkillManager>();
    }

    public void Initialize(ClassDefinition classDef)
    {
        currentClass = classDef;
    }

    private void UpdateFromStats(CharacterStats newStats)
    {
        moveSpeed = newStats.moveSpeed;
    }

    private void Update()
    {
        // Tách biệt rõ ràng các logic
        HandleMovementInput();
        HandleAttackInput();
        HandleSkillInput();
    }

    private void FixedUpdate()
    {
        // Chỉ di chuyển vật lý ở FixedUpdate
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    // =====================================================================
    // ## CÁC HÀM XỬ LÝ LOGIC (ĐÃ TÁI CẤU TRÚC) ##
    // =====================================================================

    /// <summary>
    /// Xử lý tất cả input liên quan đến di chuyển và animation di chuyển
    /// </summary>
    private void HandleMovementInput()
    {
        if (isAttacking)
        {
            moveInput = Vector2.zero; // Đứng yên tuyệt đối khi đang tấn công
        }
        else
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput.Normalize();

            // Chỉ lật sprite theo di chuyển khi không tấn công
            FlipSpriteBasedOnMovement();
        }

        // Cập nhật animation speed
        animator.SetFloat("Speed", moveInput.magnitude);
    }

    /// <summary>
    /// Xử lý tất cả input liên quan đến đòn đánh thường
    /// </summary>
    private void HandleAttackInput()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return)) && !isAttacking)
        {
            // Bước 1: Quyết định hướng tấn công
            // Chỉ xoay theo chuột nếu là class TẦM XA
            if (currentClass != null && currentClass.attackType == AttackType.Ranged)
            {
                FaceMouse();
            }
            // Nếu là cận chiến, hàm này không làm gì cả, giữ nguyên hướng hiện tại.

            // Bước 2: Bắt đầu tấn công
            isAttacking = true;
            animator.SetTrigger("Attack");
        }
    }

    /// <summary>
    /// Xử lý input kỹ năng
    /// </summary>
    private void HandleSkillInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            skillManager.UseSkill(0);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            skillManager.UseSkill(1);
        }
    }

    // =====================================================================
    // ## CÁC HÀM PHỤ TRỢ ##
    // =====================================================================

    /// <summary>
    /// Lật sprite chỉ dựa trên hướng di chuyển (Dành cho cận chiến và di chuyển thông thường)
    /// </summary>
    private void FlipSpriteBasedOnMovement()
    {
        if (moveInput.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput.x), 1f, 1f);
        }
    }

    /// <summary>
    /// Lật sprite để nhìn về phía con trỏ chuột (Chỉ dành cho class tầm xa khi tấn công)
    /// </summary>
    private void FaceMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePosition.x > transform.position.x)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    // =====================================================================
    // ## HÀM GỌI TỪ ANIMATION VÀ CÁC HÀM KHÁC ##
    // =====================================================================

    public void AttackFinished()
    {
        isAttacking = false;
    }

    public void AnimationTriggerAttack()
    {
        if (currentClass == null) return;

        switch (currentClass.attackType)
        {
            case AttackType.Melee:
                PerformMeleeAttack();
                break;
            case AttackType.Ranged:
                PerformRangedAttack();
                break;
        }
    }

    private void PerformMeleeAttack()
    {
        int finalDamage = (currentClass.damageType == DamageType.Physical)
                        ? playerStats.GetFinalPhysicalDamage()
                        : playerStats.GetFinalMagicalDamage();

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, currentClass.attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(finalDamage);
            }
        }
    }

    private void PerformRangedAttack()
    {
        if (currentClass.projectilePrefab == null || firePoint == null) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector2 fireDirection = (mousePosition - firePoint.position).normalized;

        GameObject projectileGO = Instantiate(currentClass.projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectileScript = projectileGO.GetComponent<Projectile>();

        int finalDamage = (currentClass.damageType == DamageType.Physical)
                        ? playerStats.GetFinalPhysicalDamage()
                        : playerStats.GetFinalMagicalDamage();

        projectileScript.Setup(finalDamage, enemyLayers, fireDirection);
    }

    // ... (Các hàm còn lại như OnDrawGizmosSelected, GetClassDefinition, OnEnable, OnDisable giữ nguyên) ...
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null || currentClass == null || currentClass.attackType != AttackType.Melee)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, currentClass.attackRange);
    }
    public ClassDefinition GetClassDefinition()
    {
        return currentClass;
    }
    void OnEnable()
    {
        PlayerStats.OnStatsCalculated += UpdateFromStats;
    }

    void OnDisable()
    {
        PlayerStats.OnStatsCalculated -= UpdateFromStats;
    }
}
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f; // Đạn tự hủy sau 3 giây nếu không trúng gì
    private int damage;
    private LayerMask enemyLayers;

    private Rigidbody2D rb;

    // Hàm này được PlayerController gọi ngay sau khi tạo ra đạn
    public void Setup(int damageAmount, LayerMask layers, Vector2 direction)
    {
        damage = damageAmount;
        enemyLayers = layers;

        rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * speed;

        // Xoay mũi tên theo hướng bay
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Tự hủy sau một khoảng thời gian
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // BƯỚC 1: KIỂM TRA VA CHẠM CÓ ĐƯỢC GHI NHẬN KHÔNG
        Debug.Log($"[Projectile] Đã va chạm với: {hitInfo.gameObject.name}");

        // BƯỚC 2: KIỂM TRA VA CHẠM CÓ ĐÚNG LAYER ENEMY KHÔNG
        if ((enemyLayers.value & (1 << hitInfo.gameObject.layer)) > 0)
        {
            Debug.Log($"[Projectile] Đối tượng {hitInfo.name} nằm trên đúng layer Enemy.");

            // BƯỚC 3: KIỂM TRA CÓ TÌM THẤY SCRIPT ENEMYCONTROLLER KHÔNG
            EnemyController enemy = hitInfo.GetComponent<EnemyController>();
            if (enemy != null)
            {
                Debug.Log($"[Projectile] Tìm thấy script EnemyController! Gây {damage} sát thương.");
                enemy.TakeDamage(damage);
            }
            else
            {
                Debug.LogError($"[Projectile] LỖI: KHÔNG tìm thấy script EnemyController trên đối tượng {hitInfo.name}!");
            }

            Destroy(gameObject);
        }
    }
}
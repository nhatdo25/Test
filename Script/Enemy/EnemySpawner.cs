// EnemySpawner.cs (phiên bản đã sửa)
using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public EnemyDefinition enemyToSpawn;
    public GameObject enemyPrefab;
    // Bỏ respawnTime ở đây vì đã chuyển vào EnemyDefinition
    // public float respawnTime = 5f; 

    void Start()
    {
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        if (enemyToSpawn != null && enemyPrefab != null)
        {
            GameObject currentEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            EnemyController enemyController = currentEnemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.Initialize(enemyToSpawn);
                enemyController.spawner = this; // <-- THAY ĐỔI QUAN TRỌNG: Gán spawner
            }
        }
    }

    public void RespawnEnemy(GameObject deadEnemy)
    {
        // Phá hủy đối tượng cũ
        Destroy(deadEnemy);
        // Bắt đầu đếm giờ hồi sinh
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        // Lấy thời gian hồi sinh từ data của enemy
        float respawnTime = enemyToSpawn.respawnTime;
        Debug.Log($"Kẻ địch sẽ hồi sinh sau {respawnTime} giây.");
        yield return new WaitForSeconds(respawnTime);
        SpawnEnemy();
    }
}
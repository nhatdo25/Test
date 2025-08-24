using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Spawn Object Skill")]
public class SpawnObjectSkillData : SkillData
{
    [Header("Spawn Settings")]
    public GameObject objectToSpawn;
    public int amount = 1; // Số lượng đối tượng
    public float spreadAngle = 45f; // Góc tỏa ra

    public override void Activate(GameObject user)
    {
        PlayerController controller = user.GetComponent<PlayerController>();
        if (controller == null || controller.firePoint == null) return;

        Debug.Log($"Skill '{skillName}' activated, spawning {amount} of {objectToSpawn.name}.");

        float angleStep = (amount > 1) ? spreadAngle / (amount - 1) : 0;
        float startingAngle = -spreadAngle / 2;

        for (int i = 0; i < amount; i++)
        {
            float currentAngle = startingAngle + (angleStep * i);
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle) * controller.firePoint.rotation;

            Instantiate(objectToSpawn, controller.firePoint.position, rotation);
        }
    }
}

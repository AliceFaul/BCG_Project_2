using System.Collections;
using UnityEngine;

public class SkillExecutor : MonoBehaviour
{
    public void ExecuteSkill(SkillData data, Vector3 spawnPos)
    {
        switch (data.type)
        {
            case SkillType.Kunai:
                SpawnKunai(spawnPos);
                break;

            case SkillType.Shuriken:
                SpawnShuriken(spawnPos);
                break;

            case SkillType.Cut:
                StartCoroutine(CutAttack());
                break;

            case SkillType.Heal:
                HealPlayer();
                break;

            case SkillType.Defense:
                ActivateDefense();
                break;

            case SkillType.Mist:
                SpawnMist(spawnPos);
                break;

            case SkillType.OrbFire:
                ShootFireBall(spawnPos);
                break;

            case SkillType.OrbWater:
                SpawnWaterColumn(spawnPos);
                break;

            case SkillType.RockSpike:
                SpawnRockSpike(spawnPos);
                break;
        }
    }

    // ======== IMPLEMENT TỪNG SKILL ========
    void SpawnKunai(Vector3 pos)
    {
        // Tạo Prefab + gán damage + velocity v.v…
        Debug.Log("Kunai!");
    }

    void SpawnShuriken(Vector3 pos)
    {
        Debug.Log("Ném Shuriken!");
    }

    IEnumerator CutAttack()
    {
        Debug.Log("Chém trong 3s!");
        yield return new WaitForSeconds(3f);
    }

    void HealPlayer()
    {
        Debug.Log("Hồi máu!");
    }

    void ActivateDefense()
    {
        Debug.Log("Miễn sát thương 10s!");
    }

    void SpawnMist(Vector3 pos)
    {
        Debug.Log("Tạo sương mù giữ enemy đứng yên!");
    }

    void ShootFireBall(Vector3 pos)
    {
        Debug.Log("Bắn quả cầu lửa!");
    }

    void SpawnWaterColumn(Vector3 pos)
    {
        Debug.Log("Triệu hồi cột nước!");
    }

    void SpawnRockSpike(Vector3 pos)
    {
        Debug.Log("Triệu hồi gai đá!");
    }
}
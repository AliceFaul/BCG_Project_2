using _Project._Scripts.Enemies;
using _Project._Scripts.Player;
using System.Collections;
using UnityEngine;

public class SkillExecutor : MonoBehaviour
{
    public PlayerHealth playerHealth;

    [Header("Projectile Prefabs")]
    public GameObject fireballPrefab;

    [Header("Projectile Prefabs")]
    public GameObject rockSpikePrefab;

    public void ExecuteSkill(SkillData data, Vector3 spawnPos)
    {
        Debug.Log("ExecuteSkill called: " + data.skillName + " | Type: " + data.type);
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
        Debug.Log("chay ham HealPlayer");
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(-50f); // hồi 50 máu
            Debug.Log("Đã hồi 50 máu cho player!");
        }
        else
        {
            Debug.LogWarning("PlayerHealth chưa được gán!");
        }
    }

    void ActivateDefense()
    {
        if (playerHealth != null)
        {
            playerHealth.SetInvincible(10f); // miễn sát thương 10 giây
            Debug.Log("Bật Defense: miễn sát thương 10s");
        }
    }

    void SpawnMist(Vector3 pos)
    {
        Debug.Log("Tạo sương mù! Freeze toàn bộ enemy 5s");

        // Tìm toàn bộ enemy
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyAI ai = enemy.GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.SetFrozen(5f); // đóng băng 5 giây
            }
        }

        // Nếu có hiệu ứng sương mù
        // Instantiate(mistPrefab, pos, Quaternion.identity);
    }

    void ShootFireBall(Vector3 pos)
    {
        if (fireballPrefab != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, pos, Quaternion.identity);

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            Vector3 dir = (mousePos - pos).normalized;

            FireBall fb = fireball.GetComponent<FireBall>();
            if (fb != null)
            {
                fb.Init(dir);
            }
            else
            {
                Debug.LogWarning("Prefab fireball chưa attach script Fireball!");
            }
        }
    }

   void SpawnWaterColumn(Vector3 pos)
    {
        Debug.Log("Triệu hồi cột nước!");

        // Tìm tất cả enemy trong scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyAI ai = enemy.GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.SetFrozen(10f); // freeze 10 giây
            }
        }

        // Nếu có prefab VFX cột nước
        // Instantiate(waterColumnPrefab, pos, Quaternion.identity);
    }
    void SpawnRockSpike(Vector3 pos)
    {
        Debug.Log("Triệu hồi gai đá!");

        if (rockSpikePrefab != null)
        {
            GameObject spike = Instantiate(rockSpikePrefab, pos, Quaternion.identity);

            RockSpike spikeScript = spike.GetComponent<RockSpike>();
            if (spikeScript != null)
            {
                spikeScript.Init(50f); // gán damage trước
            }
            else
            {
                Debug.LogWarning("Prefab RockSpike chưa attach script RockSpike!");
            }

            Destroy(spike, 5f);
        }
    }

}
using _Project._Scripts.Enemies;
using _Project._Scripts.Player;
using _Project._Scripts.Gameplay;
using System.Collections;
using UnityEngine;
using _Project._Scripts.Core;

public class SkillExecutor : MonoBehaviour
{
    PlayerHealth playerHealth;
    PlayerEnergy _energy;
    PlayerStats _stats;

    [Header("Projectile Prefabs")]
    public GameObject fireballPrefab;

    [Header("Projectile Prefabs")]
    public GameObject rockSpikePrefab;
    public GameObject _mistPrefab;
    public GameObject _fogPuffPrefab;

    private void Start()
    {
        _stats = GetComponent<PlayerStats>();
        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
        }

        if(_energy == null)
        {
            _energy = GetComponent<PlayerEnergy>();
        }
    }

    public void ExecuteSkill(SkillData data, Vector3 spawnPos)
    {
        Debug.Log("ExecuteSkill called: " + data.skillName + " | Type: " + data.type);
        switch (data.type)
        {
            case SkillType.Kunai:
                if (_energy._currentEnergy >= data._energyAmount)
                {
                    SoundEffectManager.Instance.Play("Whoosh");
                    _energy.ChangeEnergy(-data._energyAmount);
                    SpawnKunai(spawnPos);
                }
                break;

            case SkillType.Shuriken:
                if (_energy._currentEnergy >= data._energyAmount)
                {
                    SoundEffectManager.Instance.Play("Whoosh");
                    _energy.ChangeEnergy(-data._energyAmount);
                    SpawnShuriken(spawnPos);
                }
                break;

            case SkillType.Cut:
                if (_energy._currentEnergy >= data._energyAmount)
                {
                    SoundEffectManager.Instance.Play("Whoosh");
                    _energy.ChangeEnergy(-data._energyAmount);
                    StartCoroutine(CutAttack());
                }
                break;

            case SkillType.Heal:
                if (_energy._currentEnergy >= data._energyAmount)
                {
                    SoundEffectManager.Instance.Play("Success");
                    _energy.ChangeEnergy(-data._energyAmount);
                    HealPlayer();
                }
                break;

            case SkillType.Defense:
                if (_energy._currentEnergy >= data._energyAmount)
                {
                    SoundEffectManager.Instance.Play("Success");
                    _energy.ChangeEnergy(-data._energyAmount);
                    ActivateDefense();
                }
                break;

            case SkillType.Mist:
                if (_energy._currentEnergy >= data._energyAmount)
                {
                    _energy.ChangeEnergy(-data._energyAmount);
                    SpawnMist(transform);
                }
                break;

            case SkillType.OrbFire:
                if (_energy._currentEnergy >= data._energyAmount)
                {
                    SoundEffectManager.Instance.Play("Whoosh");
                    _energy.ChangeEnergy(-data._energyAmount);
                    ShootFireBall(spawnPos);
                }
                break;

            case SkillType.OrbWater:
                if (_energy._currentEnergy >= data._energyAmount)
                {
                    SoundEffectManager.Instance.Play("Whoosh");
                    _energy.ChangeEnergy(-data._energyAmount);
                    SpawnWaterColumn(spawnPos);
                }
                break;

            case SkillType.RockSpike:
                if (_energy._currentEnergy >= data._energyAmount)
                {
                    SoundEffectManager.Instance.Play("Whoosh");
                    _energy.ChangeEnergy(-data._energyAmount);
                    SpawnRockSpike(spawnPos);
                }
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
        float maxHealth = playerHealth._maxHealth;
        float healAmountPercent = _stats.SkillDamage * 0.02f;
        float healAmount = maxHealth * healAmountPercent;

        Debug.Log("chay ham HealPlayer");
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(-healAmount); // hồi 50 máu
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

    void SpawnMist(Transform playerTransform)
    {
        if (_mistPrefab == null)
        {
            Debug.LogWarning("_mistPrefab chưa được gán!");
            return;
        }

        Debug.Log("Tạo sương mù giữ enemy đứng yên!");
        Instantiate(_mistPrefab, transform.position, Quaternion.identity);
    }


    void ShootFireBall(Vector3 pos)
    {
        if (fireballPrefab != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, pos, Quaternion.identity);

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            Vector3 dir = (mousePos - pos).normalized;

            float baseDamage = 30f;
            float scalingPercent = .5f;
            float finalDamage = baseDamage * (1 + _stats.SkillDamage * scalingPercent / 100f);

            FireBall fb = fireball.GetComponent<FireBall>();
            if (fb != null)
            {
                fb.Init(dir, finalDamage);
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

            float baseDamage = 50f;
            float scalingPercent = .6f;
            float finalDamage = baseDamage * (1 + _stats.SkillDamage * scalingPercent / 100f);

            RockSpike spikeScript = spike.GetComponent<RockSpike>();
            if (spikeScript != null)
            {
                spikeScript.Init(finalDamage); // gán damage trước
            }
            else
            {
                Debug.LogWarning("Prefab RockSpike chưa attach script RockSpike!");
            }

            Destroy(spike, 5f);
        }
    }

}
using _Project._Scripts.Enemies.Bosses;
using UnityEngine;

public class BossRespawnController : MonoBehaviour
{
    public BossController boss;
    public BossHealth bossHealth;

    public float respawnTime = 3f;
    private float timer;
    private bool waiting = false;

    private void Start()
    {
        bossHealth.OnBossDead += StartRespawn;
    }

    private void Update()
    {
        if (!waiting) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            waiting = false;
            RespawnBoss();
        }
    }

    private void StartRespawn()
    {
        waiting = true;
        timer = respawnTime;
    }

    private void RespawnBoss()
    {
        boss.gameObject.SetActive(true);

        boss.ResetBoss();
        bossHealth.FullHeal();

        Debug.Log("BOSS RESPAWNED");
    }
}

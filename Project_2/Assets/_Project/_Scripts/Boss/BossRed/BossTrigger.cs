using UnityEngine;

namespace _Project._Scripts.Enemies.Bosses
{
    public class BossTrigger : MonoBehaviour
    {
        BossController boss;
        BossHealth bossHealth;

        private void Start()
        {
            boss = FindAnyObjectByType<BossController>();
            bossHealth = FindAnyObjectByType<BossHealth>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                boss.ActivateBoss();
                bossHealth.ShowHealth();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                boss.StopBoss();
                bossHealth.HideHealth();
            }
        }
    }
}

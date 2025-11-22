using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public BossController boss;
    public BossHealth bossHealth;

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

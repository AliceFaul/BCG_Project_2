using UnityEngine;

public class FrogTrigger : MonoBehaviour
{
    public FrogController boss;
    public FrogHealth bossHealth;   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            boss.OnPlayerEnter();
            bossHealth.ShowHealth();   // bật UI máu
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            boss.OnPlayerExit();
            bossHealth.HideHealth();   // tắt UI máu
        }
    }
}

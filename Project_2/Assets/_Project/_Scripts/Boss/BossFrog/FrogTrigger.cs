using UnityEngine;

namespace _Project._Scripts.Enemies.Bosses
{
    public class FrogTrigger : MonoBehaviour
    {
        FrogController boss;
        FrogHealth bossHealth;

        private void Start()
        {
            bossHealth = FindAnyObjectByType<FrogHealth>();
            boss = FindAnyObjectByType<FrogController>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                boss.OnPlayerEnter();
                bossHealth.ShowHealth();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // Nếu player rời vùng → boss về chỗ cũ
            if (collision.CompareTag("Player"))
            {
                boss.OnPlayerExit();
                bossHealth.HideHealth();
            }

            // Nếu chính con boss rời vùng → boss về chỗ cũ
            if (collision.CompareTag("Boss"))
            {
                Debug.Log("Boss tự ra khỏi vùng → quay về startPos");
                boss.ForceReturnToStart();
            }
        }
    }
}

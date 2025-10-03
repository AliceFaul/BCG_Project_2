using UnityEngine;

namespace _Project._Scripts.Enemies
{
    public class EnemyAI : MonoBehaviour
    {
        EnemyHealth health;

        [Header("Cấu hình")]
        [SerializeField] private float moveSpeed = 2f;   // tốc độ di chuyển
        [SerializeField] private Rigidbody2D rb;         // rigidbody để di chuyển

        private Transform targetPlayer; // player khi phát hiện
        private bool isChasing = false; // trạng thái có đang dí không
        private bool _isDead = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<EnemyHealth>();

            if (health != null)
            {
                health.OnDead += StopMoving;
                Debug.Log("Subscribe OnDead!!");
            }
        }

        private void Update()
        {
            if(_isDead) return;

            if (isChasing && targetPlayer != null)
            {
                // hướng tới player
                Vector2 direction = (targetPlayer.position - transform.position).normalized;
                rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
            }
        }

        void StopMoving()
        {
            _isDead = true;
        }

        // Player bước vào vùng VisionRange
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_isDead) return;

            if (collision.CompareTag("Player"))
            {
                targetPlayer = collision.transform;
                isChasing = true;
            }
        }

        // Player bước ra khỏi vùng VisionRange
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (_isDead) return;

            if (collision.CompareTag("Player"))
            {
                targetPlayer = null;
                isChasing = false;
            }
        }
    }
}

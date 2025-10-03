using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    [Header("Cấu hình")]
    [SerializeField] private float moveSpeed = 2f;   // tốc độ di chuyển
    [SerializeField] private Rigidbody2D rb;         // rigidbody để di chuyển

    private Transform targetPlayer; // player khi phát hiện
    private bool isChasing = false; // trạng thái có đang dí không

    [Header("Máu Enemy")]
    [SerializeField] private int maxHealth = 100;   // máu tối đa
    private int currentHealth;                      // máu hiện tại
    [SerializeField] private HealthBar healthBar;   // script thanh máu

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // khởi tạo máu ban đầu
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    private void Update()
    {
        if (isChasing && targetPlayer != null)
        {
            // hướng tới player
            Vector2 direction = (targetPlayer.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
        }
    }

    // Player bước vào vùng VisionRange
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            targetPlayer = collision.transform;
            isChasing = true;
        }
    }

    // Player bước ra khỏi vùng VisionRange
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            targetPlayer = null;
            isChasing = false;
        }
    }

    // Enemy bị đánh
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy chết!");
        Destroy(gameObject);
    }
}

using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    public Vector2 direction;
    public float damage = 10f; // sát thương viên đạn

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra va chạm với Player
        if (collision.CompareTag("Player"))
        {
            // Lấy component có TakeDamage
            var playerHealth = collision.GetComponent<IDamageable>();
            if (playerHealth != null)
            {
                // Gọi hàm TakeDamage của player
                playerHealth.TakeDamage(damage); // damage dương
            }

            Destroy(gameObject); // hủy viên đạn sau khi trúng
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}

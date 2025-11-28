using UnityEngine;

public class KunaiDamage : MonoBehaviour
{
    public float damage = 30f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDamageable enemy))
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject); // Sau khi gây damage thì biến mất
        }
    }
}

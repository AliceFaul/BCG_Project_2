using _Project._Scripts.Enemies;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed = 10f;
    public float damage;
    private Vector3 direction;

    public void Init(Vector3 dir, float finalDamage)
    {
        direction = dir.normalized;
        damage = finalDamage;
        Destroy(gameObject, 3f); // tự hủy sau 5s nếu chưa chạm
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
           
            var enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            { 
                Debug.Log("da va cham voi enemy");
                enemyHealth.TakeDamage(-damage);
            }
            Destroy(gameObject); // hủy fireball sau khi va chạm
        }
    }
}

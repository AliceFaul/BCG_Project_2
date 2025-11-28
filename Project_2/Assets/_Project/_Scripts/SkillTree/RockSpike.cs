using _Project._Scripts.Enemies;
using UnityEngine;

public class RockSpike : MonoBehaviour
{
    private float damage;

    public void Init(float dmg)
    {
        damage = dmg;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(-damage);
                Debug.Log("Gai ðá gây sát thýõng: " + damage);
            }
        }
    }
}

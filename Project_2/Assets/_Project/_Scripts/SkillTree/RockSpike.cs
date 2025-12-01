using _Project._Scripts.Enemies;
using UnityEngine;

public class RockSpike : MonoBehaviour
{
    private float damage;

    [SerializeField] private float _knockbackForce;
    [SerializeField] private float _stunTime;

    public void Init(float dmg)
    {
        damage = dmg;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable enemyHealth = other.GetComponent<IDamageable>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(-damage);
                IKnockbacked knockbacked = other.GetComponent<IKnockbacked>();
                if(knockbacked != null)
                {
                    knockbacked.Knockback(transform, _knockbackForce, _stunTime);
                }
                Debug.Log("Gai ðá gây sát thýõng: " + damage);
            }
        }
    }
}

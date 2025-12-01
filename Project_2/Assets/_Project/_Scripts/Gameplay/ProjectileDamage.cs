using UnityEngine;
using _Project._Scripts.Player;
using _Project._Scripts.Core;
using _Project._Scripts.UI;

namespace _Project._Scripts.Gameplay
{
    public class ProjectileDamage : MonoBehaviour
    {
        PlayerStats _stats;

        [SerializeField] private float _distance;
        [SerializeField] private LayerMask _enemyLayer;

        [SerializeField] private float _knockbackForce;
        [SerializeField] private float _stunTime;

        void Start()
        {
            _stats = FindAnyObjectByType<PlayerStats>();
        }

        public void DealDamage()
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _distance, _enemyLayer);

            foreach (var hit in hitColliders)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    bool isCritDamage = Random.Range(0, 100) < 20;
                    float damage = _stats.SkillDamage;

                    if(isCritDamage) damage *= 2f;

                    damageable.TakeDamage(-Mathf.Round(damage));
                    SoundEffectManager.Instance.Play("Slash");
                    DamagePopup.CreatePopup(hit.transform.position, Mathf.Round(damage), isCritDamage);
                    IKnockbacked knockbacked = hit.GetComponent<IKnockbacked>();
                    if (knockbacked != null)
                    {
                        knockbacked.Knockback(transform, _knockbackForce, _stunTime);
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _distance);
        }
    }
}

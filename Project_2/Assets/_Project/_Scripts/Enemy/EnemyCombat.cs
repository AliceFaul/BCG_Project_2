using UnityEngine;

namespace _Project._Scripts.Enemies
{
    public class EnemyCombat : MonoBehaviour
    {
        [Header("Cấu hình combat của enemy")]
        [SerializeField] private Transform _weaponPoint; //Điểm tấn công của enemy
        [SerializeField] private float _weaponRange; //Tầm tấn công
        [SerializeField] private float _damage = 15f; //Sát thương đòn đánh
        [SerializeField] private float _knockBackForce = 5f; //Lực đẩy lùi khi đánh
        [SerializeField] private float _stunTime = 0.5f;
        [SerializeField] private LayerMask _playerMask; //Layer của player

        //Hàm gọi trong Animation Event
        public void DeadDamage()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(_weaponPoint.position, _weaponRange, _playerMask);

            foreach (var hit in hits)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(-_damage);
                    IKnockbacked knockbacked = hit.GetComponent<IKnockbacked>();
                    if(knockbacked != null)
                    {
                        knockbacked.Knockback(transform, _knockBackForce, _stunTime);
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.brown;
            Gizmos.DrawWireSphere(_weaponPoint.position, _weaponRange);
        }
    }
}
using _Project._Scripts.UI;
using UnityEngine;

namespace _Project._Scripts.Enemies
{
    public class EnemyCombat : MonoBehaviour
    {
        EnemyInfo _info;

        [Header("Cấu hình combat của enemy")]
        [SerializeField] private Transform _weaponPoint; //Điểm tấn công của enemy
        [SerializeField] private float _weaponRange; //Tầm tấn công
        [SerializeField] private float _damage; //Sát thương đòn đánh
        [SerializeField] private float _knockBackForce = 5f; //Lực đẩy lùi khi đánh
        [SerializeField] private float _stunTime = 0.5f;
        [SerializeField] private LayerMask _playerMask; //Layer của player

        private void Start()
        {
            _info = GetComponent<EnemyInfo>();

            if(_info != null)
            {
                EnemyStats stats = _info._enemyData.GetStatsAfterGrowth(HUDController.Instance._currentLevel);

                SetEnemyDamage(stats);
            }
        }

        private void OnEnable()
        {
            if (_info != null)
            {
                if(gameObject.GetComponent<IDungeonEnemy>() != null) return;

                EnemyStats stats = _info._enemyData.GetStatsAfterGrowth(HUDController.Instance._currentLevel);
                SetEnemyDamage(stats);
            }
        }

        public void SetEnemyDamage(EnemyStats stats)
        {
            _damage = stats._enemyDamage;
        }

        //Hàm gọi trong Animation Event
        public void DeadDamage()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(_weaponPoint.position, _weaponRange, _playerMask);

            foreach (var hit in hits)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(_damage);
                    DamagePopup.CreatePopup(hit.transform.position, _damage, false);
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
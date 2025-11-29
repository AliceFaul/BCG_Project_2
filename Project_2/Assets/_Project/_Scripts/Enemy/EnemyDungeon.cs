using UnityEngine;

namespace _Project._Scripts.Enemies
{
    public class EnemyDungeon : MonoBehaviour, IDungeonEnemy, IEnemyDataHolder
    {
        [SerializeField] private EnemyData _data;
        EnemyStats _currentStats;
        Vector3 _spawnPosition;
        EnemyHealth _health;
        EnemyCombat _damage;

        private void Awake()
        {
            _spawnPosition = transform.position;
            _health = GetComponent<EnemyHealth>();
            _damage = GetComponent<EnemyCombat>();
        }

        public EnemyData GetEnemyData() => _data;

        public void ApplyStats(EnemyStats stats)
        {
            _currentStats = stats;

            if(_health != null)
                _health.SetMaxHealth(stats);

            if(_damage != null)
                _damage.SetEnemyDamage(stats);
        }

        public void ResetForDungeon()
        {
            transform.position = _spawnPosition;

            if(_health != null)
            {
                _health.ReviveInDungeon();
            }

            var rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector2.zero;
        }
    }
}

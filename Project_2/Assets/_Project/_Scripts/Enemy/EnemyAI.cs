using NUnit.Framework.Constraints;
using UnityEngine;

namespace _Project._Scripts.Enemies
{
    [RequireComponent(typeof(Rigidbody2D), typeof(EnemyState))]
    public class EnemyAI : MonoBehaviour
    {
        EnemyHealth health;
        [SerializeField] private EnemyState _state;

        [Header("Cấu hình Movement")]
        [SerializeField] private float moveSpeed = 2f;   // tốc độ di chuyển
        [SerializeField] private Rigidbody2D rb;         // rigidbody để di chuyển
        private Transform targetPlayer; // player khi phát hiện

        [Header("Cấu hình Attack")]
        [SerializeField] private Transform _detectPoint; //Biến transform để tìm kiếm player khi phát hiện
        [SerializeField] private float _detectRange = 5f; //Khoảng cách tầm nhìn của detectPoint
        [SerializeField] private float _attackRange = 5f; //Khoảng cách có thể tấn công
        [SerializeField] private LayerMask _playerLayer; //Layer mà player đang dùng
        [SerializeField] private float _attackCD = 2f; //Cooldown mỗi lần tấn công
        private float _attackTimer; //Bộ đếm thời gian cooldown

        //Các cờ quản lý trạng thái
        private bool _isDead = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<EnemyHealth>();

            if (health != null)
            {
                health.OnDead += StopMoving;
                Debug.Log("Subscribe OnDead!!");
            }
        }

        private void Update()
        {
            if(_isDead) return;

            if(_attackTimer > 0f)
            {
                _attackTimer -= Time.deltaTime;
            }
            CheckForPlayer();

            if (_state == EnemyState.Moving)
            {
                Chase();
            }
            else if(_state == EnemyState.Attacking)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        void StopMoving()
        {
            _isDead = true;
        }

        #region Enemy movement, Check for Player

        void Chase()
        {
            Vector2 dir = (targetPlayer.position - transform.position).normalized;
            rb.linearVelocity = dir * moveSpeed;
        }

        void CheckForPlayer()
        {
            Collider2D[] players = Physics2D.OverlapCircleAll(_detectPoint.position, _detectRange, _playerLayer);

            if(players.Length > 0)
            {
                targetPlayer = players[0].transform;

                if (Vector2.Distance(transform.position, targetPlayer.position) <= _attackRange && _attackTimer <= 0)
                {
                    _attackTimer = _attackCD;
                    ChangeState(EnemyState.Attacking);
                }
                else if(Vector2.Distance(transform.position, targetPlayer.position) > _attackRange)
                {
                    ChangeState(EnemyState.Moving);
                }
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                ChangeState(EnemyState.Idle);
            }
        }

        #endregion

        void ChangeState(EnemyState newState)
        {
            _state = newState;
        }

        private void OnDrawGizmosSelected()
        {
            if(_detectPoint == null) return;

            Gizmos.color = Color.violet;
            Gizmos.DrawWireSphere(_detectPoint.position, _detectRange);
        }
    }

    public enum EnemyState { Idle, Moving, Attacking }
}

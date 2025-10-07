using System.Collections;
using UnityEngine;

namespace _Project._Scripts.Enemies
{
    [RequireComponent(typeof(Rigidbody2D), typeof(EnemyState), typeof(Animator))]
    public class EnemyAI : MonoBehaviour, IKnockbacked
    {
        EnemyHealth health;
        [SerializeField] private EnemyState _state;
        private Animator _anim;

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

        #region Unity Life Cycle

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<EnemyHealth>();
            _anim = GetComponent<Animator>();

            if (health != null)
            {
                health.OnDead += StopMoving;
                Debug.Log("Subscribe OnDead!!");
            }
        }

        private void Update()
        {
            if(_isDead) return;

            if (_state != EnemyState.Knockbacked)
            {
                if (_attackTimer > 0f)
                {
                    _attackTimer -= Time.deltaTime;
                }
                CheckForPlayer();

                if (_state == EnemyState.Moving)
                {
                    Chase();
                }
                else if (_state == EnemyState.Attacking)
                {
                    rb.linearVelocity = Vector2.zero;
                }
            }
        }

        void StopMoving()
        {
            _isDead = true;
        }

        #endregion

        #region Enemy movement, Check for Player

        //Hàm truy đuổi, lấy theo hướng của Player
        void Chase()
        {
            Vector2 dir = (targetPlayer.position - transform.position).normalized;
            rb.linearVelocity = dir * moveSpeed;
        }

        //Hàm cốt lõi tìm kiếm player, tính toán khoảng cách để chuyển state
        void CheckForPlayer()
        {
            Collider2D[] players = Physics2D.OverlapCircleAll(_detectPoint.position, _detectRange, _playerLayer);

            //Nếu tìm thấy player
            if(players.Length > 0)
            {
                targetPlayer = players[0].transform;

                //Nếu khoảng cách giữa player và transform bé hơn tầm đánh và cooldown ok thì chuyển sang state attack
                if (Vector2.Distance(transform.position, targetPlayer.position) <= _attackRange && _attackTimer <= 0)
                {
                    _attackTimer = _attackCD;
                    ChangeState(EnemyState.Attacking);
                }
                //Nếu vẫn chưa vào tầm đánh thì sẽ tiếp tục truy đuổi
                else if(Vector2.Distance(transform.position, targetPlayer.position) > _attackRange && _state != EnemyState.Attacking)
                {
                    ChangeState(EnemyState.Moving);
                }
            }
            //Nếu không tìm thấy thì đứng yên
            else
            {
                rb.linearVelocity = Vector2.zero;
                ChangeState(EnemyState.Idle);
            }
        }

        #endregion

        #region Enemy State Animation Controller
        
        /// <summary>
        /// Hàm chuyển state và những animation đi theo state đó, sử dụng enum để chuyển state hiệu quả hơn bool
        /// </summary>
        void ChangeState(EnemyState newState)
        {
            if (_state == EnemyState.Idle)
                _anim.SetBool("isIdle", false);
            else if (_state == EnemyState.Moving)
                _anim.SetBool("isWalk", false);
            else if (_state == EnemyState.Attacking)
                _anim.SetBool("isAttacking", false);

            _state = newState;

            if (_state == EnemyState.Idle)
            {
                _anim.SetBool("isIdle", true);
                _anim.SetFloat("LastHorizontal", rb.linearVelocity.x);
                _anim.SetFloat("LastVertical", rb.linearVelocity.y);
            }
            else if (_state == EnemyState.Moving)
            {
                _anim.SetBool("isWalk", true);
                _anim.SetFloat("Horizontal", rb.linearVelocity.x);
                _anim.SetFloat("Vertical", rb.linearVelocity.y);
            }
            else if (_state == EnemyState.Attacking)
            {
                Vector2 dir = (targetPlayer.position - transform.position).normalized;
                dir.x = Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? Mathf.Sign(dir.x) : 0;
                dir.y = Mathf.Abs(dir.y) > Mathf.Abs(dir.x) ? Mathf.Sign(dir.y) : 0;
                _anim.SetBool("isAttacking", true);
                _anim.SetFloat("PlayerDirX", dir.x);
                _anim.SetFloat("PlayerDirY", dir.y);
            }
        }

        #endregion

        #region Enemy Other Setting

        public void Knockback(Transform obj, float knockbackForce, float stunTime)
        {
            ChangeState(EnemyState.Knockbacked);
            StartCoroutine(StunTime(stunTime));
            Vector2 dir = (transform.position - obj.position).normalized;
            rb.linearVelocity = dir * knockbackForce;
            Debug.Log("Enemy has knockbacked");
        }

        IEnumerator StunTime(float stunTime)
        {
            yield return new WaitForSeconds(.15f);
            rb.linearVelocity = Vector2.zero;
            yield return new WaitForSeconds(stunTime);
            ChangeState(EnemyState.Idle);
        }

        private void OnDrawGizmosSelected()
        {
            if(_detectPoint == null) return;

            Gizmos.color = Color.violet;
            Gizmos.DrawWireSphere(_detectPoint.position, _detectRange);
        }

        #endregion
    }

    public enum EnemyState { Idle, Moving, Attacking, Knockbacked }
}

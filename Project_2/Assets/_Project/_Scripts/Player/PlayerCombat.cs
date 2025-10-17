using _Project._Scripts.Core;
using _Project._Scripts.UI;
using UnityEngine;

namespace _Project._Scripts.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Các biến tham chiếu")]
        [SerializeField] private Transform _attackPoint;
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private AttackDirection _attackDir;

        [Space(10)]

        [Header("Các biến thiết lập")]
        [Tooltip("Dùng để gây sát thương")]
        [SerializeField] private float _damage = 15f;
        [SerializeField] private float _attackRange = 15f;
        [SerializeField] private float _moveOffset = 0.2f;
        [Tooltip("Lực đẩy khi tấn công")]
        [SerializeField] private float _knockbackForce = 10f; //Lực đẩy khi đánh trúng enemies
        [Tooltip("Thời gian bị stun của enemies bị đánh trúng")]
        [SerializeField] private float _stunTime = 1f; //Thời gian bị stun của enemies bị đánh trúng

        // Update is called once per frame
        void Update()
        {
            if (PauseController.IsGamePaused)
                return;

            MoveAttackPoint();
        }

        #region Setting Attack point

        //Hàm di chuyển attack point theo hướng tấn công
        void MoveAttackPoint()
        {
            Vector3 player = transform.position;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            Vector2 dir = (mousePosition - player).normalized;
            _attackDir = GetDirection(dir);
            
            switch(_attackDir)
            {
                case AttackDirection.Up:
                    _attackPoint.position = new Vector3(player.x, player.y + _moveOffset, player.z);
                    break;
                case AttackDirection.Left:
                    _attackPoint.position = new Vector3(player.x - _moveOffset, player.y, player.z);
                    break;
                case AttackDirection.Right:
                    _attackPoint.position = new Vector3(player.x + _moveOffset, player.y, player.z);
                    break;
                case AttackDirection.Down:
                    _attackPoint.position = new Vector3(player.x, player.y - _moveOffset, player.z);
                    break;
                case AttackDirection.Center:
                    _attackPoint.position = player;
                    break;
            }
        }

        //Hàm lấy hướng tấn công
        AttackDirection GetDirection(Vector2 input)
        {
            //if (input == Vector2.up) return AttackDirection.Up;
            //else if (input == Vector2.left) return AttackDirection.Left;
            //else if (input == Vector2.right) return AttackDirection.Right;
            //else if (input == Vector2.down) return AttackDirection.Down;
            //else return AttackDirection.Center;

            if(Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                return input.x > 0 ? AttackDirection.Right : AttackDirection.Left;
            }
            else
            {
                return input.y > 0 ? AttackDirection.Up : AttackDirection.Down;
            }
        }

        #endregion

        //Hàm gọi trong Animation Event
        public void Attack()
        {
            if (_attackPoint == null)
            {
                Debug.Log("Attack Point null");
                return;
            }

            Collider2D[] hits = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRange, _enemyLayer);

            foreach(var hit in hits)
            {
                //Tham chiếu hàm trừ máu của Enemy
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if(damageable != null)
                {
                    damageable.TakeDamage(-_damage);
                    SoundEffectManager.Play("Slash");
                    Debug.Log("Enemy take damage");
                    DamagePopup.CreatePopup(hit.transform.position, _damage, false);
                    IKnockbacked knockbacked = hit.GetComponent<IKnockbacked>();
                    if (knockbacked != null)
                    {
                        knockbacked.Knockback(transform, _knockbackForce, _stunTime);
                    }
                }
            }
        }

        //Thiết kế trực quan trong scene view để điều chỉnh
        private void OnDrawGizmosSelected()
        {
            if (_attackPoint == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);
        }
    }

    public enum AttackDirection { Left, Right, Up, Down, Center }
}

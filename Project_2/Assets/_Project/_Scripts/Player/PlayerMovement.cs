using System.Collections;
using UnityEngine;

namespace _Project._Scripts.Player
{
    //Script Di chuyển và điều khiển toàn bộ animation của Player
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PlayerState))]
    public class PlayerMovement : MonoBehaviour, IKnockbacked
    {
        [Header("Các biến movement")]
        [SerializeField] private float _moveSpeed = 3f;

        [Header("Các biến trạng thái")]
        [Tooltip("Thông số dùng cho việc di chuyển")]
        private Rigidbody2D _rb;
        private Animator _anim;
        [SerializeField] private PlayerState _state;
        PlayerStamina _playerStamina;
        private Vector2 _moveInput;
        private Vector3 _mousePosition;
        private Vector2 _lastInput;
        private float _currentSpeed; //Tốc độ hiện tại, update trong tương lai
        [Tooltip("Thiết lập thông số attack")]
        private float _attackTimer; //Biến đếm thời gian khi cooldown hết
        [SerializeField] private float _attackCD = 2f; //Cooldown mỗi lượt đánh
        [SerializeField] private float _dashDistance = 0.5f; //Khoảng cách lướt nhẹ về phía hướng đánh
        [SerializeField] private float _attackStamina = 15f; //Lượng stamina cần để thực hiện tấn công

        //Các biến bool kiểm soát trạng thái di chuyển của Player
        [HideInInspector] public bool _canMove = false;
        [HideInInspector] public bool _canAttack = false;
        private bool _inputBuffered = false;
        private bool _isKnockbacked = false;

        private void Awake()
        {
            //Tham chiếu các component của player 
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            InitializeMovement();
        }

        // Update is called once per frame
        void Update()
        {
            //Đếm thời gian cho đến lượt tấn công tiếp theo
            if(_attackTimer >= 0)
            {
                _attackTimer -= Time.deltaTime;
            }

            if (_isKnockbacked) return;

            ProcessInput();
        }

        //Dùng FixedUpdate để xử lý ổn định di chuyển của Player
        void FixedUpdate()
        {
            if (_isKnockbacked) return;
            Movement();
        }

        //Hàm khởi tạo khi mới bắt đầu
        void InitializeMovement()
        {
            SetPlayerControl(true);
            ChangeState(PlayerState.Idle);
            _currentSpeed = _moveSpeed;
            _playerStamina = GetComponent<PlayerStamina>();
        }

        #region Input Movement
        //Hàm xử lý tất cả các Input liên quan đến di chuyển
        void ProcessInput()
        {
            if (PlayerInput.Instance == null) return;

            //Input Di chuyển
            if (_canMove)
            {
                _moveInput = PlayerInput.Instance._moveInput;
                if (_moveInput != Vector2.zero)
                {
                    _lastInput = _moveInput;
                }
            }
            else
            {
                _moveInput = Vector2.zero;
            }

            //Input tấn công
            if (_canAttack && PlayerInput.Instance._attackInput && _attackTimer <= 0 &&
                _playerStamina._currentStamina >= _attackStamina)
            {
                ChangeState(PlayerState.Attack);
            }
        }
        #endregion

        #region Movement, Attack Event
        //Hàm di chuyển
        void Movement()
        {
            if (!_canMove || _state == PlayerState.Attack)
            {
                _rb.linearVelocity = Vector2.zero;
                return;
            }

            if (_moveInput == Vector2.zero)
            {
                ChangeState(PlayerState.Idle);
            }
            else
            {
                ChangeState(PlayerState.Walk);
            }

            _moveInput.Normalize();

            _rb.linearVelocity = _moveInput * _currentSpeed;
        }

        //Hàm gọi trong Animation Event
        public void EnableCombo()
        {
            if(_inputBuffered)
            {
                _anim.SetBool("canCombo", true);
                _inputBuffered = false;
            }
        }

        //Hàm gọi trong Animation Event
        public void DisableCombo()
        {
            _anim.SetBool("canCombo", false);
            _inputBuffered = false;
        }

        //Hàm gọi trong Animation Event
        public void EndAttack()
        {
            if (_inputBuffered) return;

            _anim.SetBool("isAttacking", false);
            ChangeState(PlayerState.Idle);
            _attackTimer = _attackCD;
        }

        #endregion

        #region Control Player State, Animation
        //Hàm điều khiển chuyển động của Player
        private void SetPlayerControl(bool _isEnabled)
        {
            _canMove = _isEnabled;
            _canAttack = _isEnabled;
        }

        void ChangeState(PlayerState _newState)
        {
            if (_anim == null) return;

            _state = _newState;

            switch (_state)
            {
                case PlayerState.Idle:
                    //Chạy animation idle của Player
                    _anim.SetBool("isMoving", false);
                    _anim.SetFloat("LastInputX", _lastInput.x);
                    _anim.SetFloat("LastInputY", _lastInput.y);
                    break;
                case PlayerState.Walk:
                    //Chạy animation walk của Player
                    _anim.SetBool("isMoving", true);
                    _anim.SetFloat("InputX", _moveInput.x);
                    _anim.SetFloat("InputY", _moveInput.y);
                    break;
                case PlayerState.Attack:
                    //Nếu chuột ở ngoài màn hình game sẽ return
                    if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width ||
                        Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
                    {
                        return;
                    }

                    //_anim.SetBool("isMoving", false);
                    _playerStamina.ChangeStamina(-_attackStamina);

                    //Lấy hướng của chuột
                    _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    _mousePosition.z = 0;
                    Vector2 dir = (_mousePosition - transform.position).normalized;

                    //Chạy animation Attack của Player
                    if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") ||
                        _anim.GetCurrentAnimatorStateInfo(0).IsName("Moving"))
                    {
                        _anim.SetBool("isAttacking", true);
                        _anim.SetFloat("MouseInputX", dir.x);
                        _anim.SetFloat("MouseInputY", dir.y);
                        _rb.MovePosition(_rb.position + _dashDistance * dir);
                    }
                    else
                    {
                        _inputBuffered = true;
                    }
                    break;
            }
        }
        #endregion

        #region Knockback, Dash Setting

        IEnumerator KnockbackCounter(float stunTime)
        {
            yield return new WaitForSeconds(stunTime);
            _rb.linearVelocity = Vector2.zero;
            _isKnockbacked = false;
        }

        public void Knockback(Transform obj, float knockbackForce, float stunTime)
        {
            _isKnockbacked = true;
            Vector2 dir = (transform.position - obj.position).normalized;
            _rb.linearVelocity = dir * knockbackForce;
            StartCoroutine(KnockbackCounter(stunTime));
            //Debug.Log("Player has knockbacked");
        }

        #endregion
    }

    public enum PlayerState { Idle, Walk, Attack }
}

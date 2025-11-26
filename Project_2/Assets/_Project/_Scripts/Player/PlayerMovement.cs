using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using _Project._Scripts.Gameplay;
using _Project._Scripts.Core;
using _Project._Scripts.UI;

namespace _Project._Scripts.Player
{
    //Script Di chuyển và điều khiển toàn bộ animation của Player
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PlayerState))]
    public class PlayerMovement : MonoBehaviour, IKnockbacked
    {
        [Header("Các biến movement")]
        [SerializeField] private float _walkSpeed = 3f;
        [SerializeField] private float _runSpeed = 6f;

        //Các biến đếm thời gian cho việc tiêu hao stamina khi player ở state Running
        private float _staminaTimer;
        private float _staminaDrainRate = .2f;

        [Header("Các biến trạng thái")]
        [Tooltip("Thông số dùng cho việc di chuyển")]
        private Rigidbody2D _rb;
        private Animator _anim;
        [SerializeField] public PlayerState _state;
        PlayerHealth _playerHealth;
        PlayerStamina _playerStamina;
        PlayerStats _stats;
        InteractionDetector _interactable;
        HUDController _hudController;
        private Vector2 _moveInput;
        private Vector3 _mousePosition;
        private Vector2 _lastInput;
        private float _currentSpeed; //Tốc độ hiện tại, update trong tương lai
        [SerializeField] private float _footstepSpeed = 1.5f;
        [Tooltip("Thiết lập thông số attack")]
        private float _attackTimer; //Biến đếm thời gian khi cooldown hết
        [SerializeField] private float _attackCD = 2f; //Cooldown mỗi lượt đánh
        [SerializeField] private float _dashDistance = 0.5f; //Khoảng cách lướt nhẹ về phía hướng đánh
        [SerializeField] private float _attackStamina = 15f; //Lượng stamina cần để thực hiện tấn công
        [SerializeField] private ParticleSystem _onLevelUpParticle;

        //Các biến bool kiểm soát trạng thái di chuyển của Player
        [HideInInspector] public bool _canMove = false;
        [HideInInspector] public bool _canAttack = false;
        private bool _inputBuffered = false;
        private bool _isKnockbacked = false;
        private bool _isPlayingFootstep = false;
        private bool _isRunning = false;

        // dùng cho skill3 của boss
        public void Freeze(float duration)
        {
            StartCoroutine(FreezeRoutine(duration));
        }

        private IEnumerator FreezeRoutine(float duration)
        {
            SetPlayerControl(false);
            Debug.Log("Player bị đóng băng " + duration + " giây");

            yield return new WaitForSeconds(duration);

            SetPlayerControl(true);
            Debug.Log("Player hết đóng băng");
        }

        private void Awake()
        {
            //Tham chiếu các component của player 
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
            _playerHealth = GetComponent<PlayerHealth>();
            _hudController = FindAnyObjectByType<HUDController>();
            _stats = GetComponent<PlayerStats>();

            if(_playerHealth != null)
            {
                _playerHealth.OnDead += () => SetPlayerControl(false);
                _playerHealth.OnRevive += () => SetPlayerControl(true);
                Debug.Log("Subcribe PlayerHealth!");
            }
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
            Movement();
        }

        //Hàm khởi tạo khi mới bắt đầu
        void InitializeMovement()
        {
            SetPlayerControl(true);
            ChangeState(PlayerState.Idle);
            _currentSpeed = _walkSpeed;
            _playerStamina = GetComponent<PlayerStamina>();
            _interactable = GetComponentInChildren<InteractionDetector>();
            
            if(_hudController != null)
            {
                _hudController.OnLevelUp += PlayerSpecialDance;
                _hudController.OnLevelUp += HandleLevelUp;
                Debug.LogWarning("Subscribe OnLevelUp");
            }
        }

        //Hàm dừng chuyển động, sử dụng PauseController
        bool PlayerInPause()
        {
            if (PauseController.IsGamePaused) return true;
            else return false;
        }

        void PlayerSpecialDance()
        {
            ChangeState(PlayerState.Special);
            PlayerLevelUpParticle();
            _rb.linearVelocity = Vector2.zero;

            Debug.LogWarning("Player in special dance");
        }

        void HandleLevelUp()
        {
            if (_stats == null) return;

            _stats.OnLevelUp();
        }

        void PlayerLevelUpParticle()
        {
            if (_onLevelUpParticle == null) return;

            ParticleSystem levelUpParticle = Instantiate(_onLevelUpParticle, transform.position, Quaternion.identity);
            levelUpParticle.transform.SetParent(transform);
        }

        #region Input Movement
        //Hàm xử lý tất cả các Input liên quan đến di chuyển
        void ProcessInput()
        {
            if (PlayerInPause()) return;

            if (PlayerInput.Instance == null) return;

            //Input Di chuyển
            if (_canMove)
            {
                _moveInput = PlayerInput.Instance._moveInput;
                _isRunning = PlayerInput.Instance._runningInput;
                if (_moveInput != Vector2.zero)
                {
                    _lastInput = _moveInput;
                }
            }
            else
            {
                _moveInput = Vector2.zero;
            }

            //(Updated) Nếu chuột click vào hay rờ vào UI rồi click thì sẽ return không attack
            if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            //Input tấn công
            if (_canAttack && PlayerInput.Instance._attackInput && _attackTimer <= 0 &&
                _playerStamina._currentStamina >= _attackStamina)
            {
                ChangeState(PlayerState.Attack);
                SoundEffectManager.Instance.Play("Whoosh");
            }

            //Input tương tác
            if(_canMove && PlayerInput.Instance._interactInput)
            {
                _interactable.OnInteract();
            }
        }
        #endregion

        #region Movement, Attack Event
        //Hàm di chuyển
        void Movement()
        {
            if(!_canMove || _state == PlayerState.Attack || PlayerInPause() || _state == PlayerState.Special)
            {
                _rb.linearVelocity = Vector2.zero;
                return;
            }

            if (_moveInput == Vector2.zero)
            {
                ChangeState(PlayerState.Idle);
                StopFootstep();
                if (_staminaTimer < _staminaDrainRate)
                    _staminaTimer = _staminaDrainRate;
            }
            else
            {
                if (_isRunning)
                {
                    ChangeState(PlayerState.Running);
                    _staminaTimer -= Time.deltaTime;
                    if(_staminaTimer <= 0f)
                    {
                        _playerStamina.ChangeStamina(-1f);
                        _staminaTimer = _staminaDrainRate;
                    }
                }
                else
                {
                    ChangeState(PlayerState.Walk);
                    if(_staminaTimer < _staminaDrainRate)
                        _staminaTimer = _staminaDrainRate;
                }
            }

            _moveInput.Normalize();

            _currentSpeed = _isRunning ? _runSpeed : _walkSpeed;
            _rb.linearVelocity = _moveInput * _currentSpeed;

            if(_rb.linearVelocity.magnitude > 0 && !_isPlayingFootstep)
            {
                StartFootstep();
            }
            else if(_rb.linearVelocity.magnitude == 0)
            {
                StopFootstep();
            }
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
            //if (_inputBuffered) return;

            _anim.SetBool("isAttacking", false);
            _inputBuffered = false;
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
            Debug.Log($"Player Control State: {_isEnabled}");
        }

        public void EndSpecialEvent()
        {
            _anim.SetBool("isSpecial", false);
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
                    _anim.SetBool("isRunning", false);
                    _anim.SetFloat("LastInputX", _lastInput.x);
                    _anim.SetFloat("LastInputY", _lastInput.y);
                    break;
                case PlayerState.Walk:
                    //Chạy animation walk của Player
                    _anim.SetBool("isMoving", true);
                    _anim.SetBool("isRunning", false);
                    _anim.SetFloat("InputX", _moveInput.x);
                    _anim.SetFloat("InputY", _moveInput.y);
                    break;
                case PlayerState.Running:
                    _anim.SetBool("isRunning", true);
                    _anim.SetFloat("InputX", _moveInput.x);
                    _anim.SetFloat("InputY", _moveInput.y);
                    break;
                case PlayerState.Special:
                    if(_anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || 
                        _anim.GetCurrentAnimatorStateInfo(0).IsName("Moving") || 
                        _anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") || 
                        _anim.GetCurrentAnimatorStateInfo(0).IsName("Running"))
                    {
                        _anim.SetBool("isSpecial", true);
                    }
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
                        _anim.GetCurrentAnimatorStateInfo(0).IsName("Moving") || 
                        _anim.GetCurrentAnimatorStateInfo(0).IsName("Running"))
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

        #region Footstep Setting

        //Hàm dừng âm thanh lại khi đứng yên
        void StopFootstep()
        {
            _isPlayingFootstep = false;
            CancelInvoke(nameof(PlayFootstep));
        }

        //Hàm chạy âm thanh lặp đi lặp lại bằng InvokeRepeating để tạo khoảng nghỉ tăng độ chân thật
        void StartFootstep()
        {
            _isPlayingFootstep = true;
            InvokeRepeating(nameof(PlayFootstep), 0f, _footstepSpeed);
        }

        void PlayFootstep()
        {
            SoundEffectManager.Instance.Play("Footstep", true);
        }

        #endregion
    }

    public enum PlayerState { Idle, Walk, Attack, Special, Running }
}

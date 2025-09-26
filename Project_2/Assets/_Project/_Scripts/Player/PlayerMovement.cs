using UnityEngine;

namespace _Project._Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Các biến movement")]
        [SerializeField] private float _moveSpeed = 3f;

        //Các biến trạng thái
        private Rigidbody2D _rb;
        private Animator _anim;
        private Vector2 _moveInput;
        private Vector2 _lastInput;
        private float _currentSpeed;

        //Các biến bool kiểm soát trạng thái di chuyển của Player
        private bool _canMove = true;

        private void Awake()
        {
            //Tham chiếu các component của player 
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _currentSpeed = _moveSpeed;
        }

        // Update is called once per frame
        void Update()
        {
            ProcessInput();
        }

        //Dùng FixedUpdate để xử lý ổn định di chuyển của Player
        void FixedUpdate()
        {
            Movement();
        }

        //Hàm xử lý tất cả các Input liên quan đến di chuyển
        void ProcessInput()
        {
            if(_canMove)
            {
                _moveInput = PlayerInput.Instance._moveInput;
                if(_moveInput != Vector2.zero)
                {
                    _lastInput = _moveInput;
                }
            }
            else
            {
                _moveInput = Vector2.zero;
            }
        }

        void Movement()
        {
            if(!_canMove)
            {
                return;
            }
            _anim.SetBool("isMoving", true);
            _anim.SetFloat("InputX", _moveInput.x);
            _anim.SetFloat("InputY", _moveInput.y);

            if(_moveInput == Vector2.zero)
            {
                _anim.SetBool("isMoving", false);
                _anim.SetFloat("LastInputX", _lastInput.x);
                _anim.SetFloat("LastInputY", _lastInput.y);
            }

            _moveInput.Normalize();

            _rb.linearVelocity = _moveInput * _currentSpeed;
        }
    }
}

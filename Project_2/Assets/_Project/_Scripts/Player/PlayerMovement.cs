using UnityEngine;

namespace _Project._Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Các biến movement")]
        [SerializeField] private float _moveSpeed = 3f;

        //Các biến trạng thái
        private Rigidbody2D _rb;
        private Vector2 _moveInput;
        private float _currentSpeed;

        //Các biến bool kiểm soát trạng thái di chuyển của Player
        private bool _canMove = true;

        private void Awake()
        {
            if(_rb == null)
            {
                _rb = GetComponent<Rigidbody2D>();
            }
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
                _moveInput.x = Input.GetAxisRaw("Horizontal");
                _moveInput.y = Input.GetAxisRaw("Vertical");
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
            _moveInput.Normalize();

            _rb.linearVelocity = _moveInput * _currentSpeed;
        }
    }
}

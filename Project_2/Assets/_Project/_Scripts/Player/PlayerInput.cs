using UnityEngine;

namespace _Project._Scripts.Player
{
    public class PlayerInput : MonoBehaviour
    {
        public static PlayerInput Instance { get; private set; }

        //Các thông số input
        public Vector2 _moveInput { get; private set; }
        public bool _attackInput { get; private set; }
        public bool _menuInput { get; private set; }
        public bool _pauseInput { get; private set; }
        public bool _infoInput { get; private set; }
        public bool _interactInput { get; private set; }
        public bool _runningInput { get; private set; }

        private void Awake()
        {
            //Tạo Singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            HandlePCInput();
        }

        #region Input Handler
        public void HandlePCInput()
        {
            _moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            _attackInput = Input.GetMouseButtonDown(0);
            _menuInput = Input.GetKeyDown(KeyCode.B);
            _infoInput = Input.GetKeyUp(KeyCode.F);
            _pauseInput = Input.GetKeyDown(KeyCode.Escape);
            _interactInput = Input.GetKeyDown(KeyCode.E);
            _runningInput = Input.GetKey(KeyCode.LeftShift);
        }
        #endregion
    }
}

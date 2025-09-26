using UnityEngine;

namespace _Project._Scripts.Player
{
    public class PlayerInput : MonoBehaviour
    {
        public static PlayerInput Instance { get; private set; }

        //Các thông số input
        public Vector2 _moveInput { get; private set; }

        private void Awake()
        {
            //Tạo Singleton
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
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
        }
        #endregion
    }
}

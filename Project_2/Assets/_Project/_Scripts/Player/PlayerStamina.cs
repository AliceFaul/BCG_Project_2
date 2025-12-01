using _Project._Scripts.UI;
using UnityEngine;

namespace _Project._Scripts.Player
{
    public class PlayerStamina : MonoBehaviour
    {
        PlayerStats _stats;
        PlayerMovement _player;

        [Header("Các thông số Stamina")]
        [SerializeField] private float _maxStamina = 100f; //Thể lực tối đa
        [SerializeField] public float _currentStamina; //Thể lực hiện tại
        private float _recoverTimer; //Bộ đếm thời gian tới lượt hồi stamina tiếp theo
        [SerializeField] private float _recoverTime = 1f; //Thời gian chờ hồi stamina
        [SerializeField] private float _recoveryAmount = 10f; //Lượng hồi stamina

        //Các biến bool kiểm soát Stamina
        private bool _canRecover = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _stats = GetComponent<PlayerStats>();
            _player = GetComponent<PlayerMovement>();
            _maxStamina = _stats.Stamina;
            _currentStamina = _maxStamina;
            HUDController.Instance.UpdateStaminaUI(_currentStamina, _maxStamina);

            _stats.OnStatChanged += UpdateMaxStamina;
            _stats.OnStatsInitialize += UpdateMaxStamina;
        }

        // Update is called once per frame
        void Update()
        {
            if(_player._state == PlayerState.Running ||
                _player._state == PlayerState.Attack)
            {
                return;
            }

            RecoveryStamina(_recoveryAmount);
        }

        void UpdateMaxStamina()
        {
            float percent = _currentStamina / _maxStamina;
            _maxStamina = _stats.Stamina;

            _currentStamina = _maxStamina * (int)percent;
            HUDController.Instance.UpdateStaminaUI(_currentStamina, _maxStamina);
        }

        #region Stamina System

        //Hàm hồi phục stamina theo thời gian
        void RecoveryStamina(float amount)
        {
            //Kiểm tra nếu stamina được hồi phục bằng với lượng stamina tối đa thì dừng hồi phục
            if (_currentStamina >= _maxStamina)
            {
                _currentStamina = _maxStamina;
                _canRecover = false;
                return;
            }

            //Đếm thời gian cho tới lượt hồi máu tiếp theo
            if(_recoverTimer >= 0 && !_canRecover)
            {
                _recoverTimer -= Time.deltaTime;
            }
            else
            {
                _canRecover = true;
            }

            //Hồi phục stamina
            if(_canRecover)
            {
                _canRecover = false;
                _currentStamina += amount;
                _recoverTimer = _recoverTime;
            }

            HUDController.Instance.UpdateStaminaUI(_currentStamina, _maxStamina);
        }

        //Hàm điều khiển stamina, nếu amount là âm sẽ tiêu hao thể lực còn dương sẽ hồi thể lực
        public void ChangeStamina(float amount)
        {
            _currentStamina += amount;
            HUDController.Instance.UpdateStaminaUI(_currentStamina, _maxStamina);

            if (_currentStamina <= 0f)
            {
                _currentStamina = 0f;
            }
        }

        #endregion
    }
}

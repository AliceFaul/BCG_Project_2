using _Project._Scripts.UI;
using UnityEngine;

namespace _Project._Scripts.Player
{
    public class PlayerEnergy : MonoBehaviour
    {
        PlayerStats _stats;

        [Header("Các thông số Stamina")]
        [SerializeField] private float _maxEnergy = 100f; //Thể lực tối đa
        [SerializeField] public float _currentEnergy; //Thể lực hiện tại
        private float _recoverTimer; //Bộ đếm thời gian tới lượt hồi stamina tiếp theo
        [SerializeField] private float _recoverTime = 1f; //Thời gian chờ hồi stamina
        [SerializeField] private float _recoveryAmount = 10f; //Lượng hồi stamina

        //Các biến bool kiểm soát Stamina
        private bool _canRecover = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _stats = GetComponent<PlayerStats>();
            _maxEnergy = _stats.Energy;
            _currentEnergy = _maxEnergy;
            HUDController.Instance.UpdateEnergyUI(_currentEnergy, _maxEnergy);

            _stats.OnStatChanged += UpdateMaxEnergy;
            _stats.OnStatsInitialize += UpdateMaxEnergy;
        }

        // Update is called once per frame
        void Update()
        {
            RecoveryEnergy(_recoveryAmount);
        }

        void UpdateMaxEnergy()
        {
            float percent = _currentEnergy / _currentEnergy;
            _maxEnergy = _stats.Energy;

            _currentEnergy = _maxEnergy * (int)percent;
            HUDController.Instance.UpdateStaminaUI(_currentEnergy, _maxEnergy);
        }

        #region Energy System

        //Hàm hồi phục stamina theo thời gian
        void RecoveryEnergy(float amount)
        {
            //Kiểm tra nếu energy được hồi phục bằng với lượng energy tối đa thì dừng hồi phục
            if (_currentEnergy >= _maxEnergy)
            {
                _currentEnergy = _maxEnergy;
                _canRecover = false;
                return;
            }

            //Đếm thời gian cho tới lượt hồi máu tiếp theo
            if (_recoverTimer >= 0 && !_canRecover)
            {
                _recoverTimer -= Time.deltaTime;
            }
            else
            {
                _canRecover = true;
            }

            //Hồi phục energy
            if (_canRecover)
            {
                _canRecover = false;
                _currentEnergy += amount;
                _recoverTimer = _recoverTime;
            }

            HUDController.Instance.UpdateEnergyUI(_currentEnergy, _maxEnergy);
        }

        //Hàm điều khiển energy, nếu amount là âm sẽ tiêu hao năng còn dương sẽ hồi năng lượng
        public void ChangeEnergy(float amount)
        {
            _currentEnergy += amount;
            HUDController.Instance.UpdateEnergyUI(_currentEnergy, _maxEnergy);

            if (_currentEnergy <= 0f)
            {
                _currentEnergy = 0f;
            }
        }

        #endregion
    }
}

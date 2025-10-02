using UnityEngine;

namespace _Project._Scripts.Player
{
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerHealth : MonoBehaviour
    {
        PlayerMovement _movement;

        [Header("Các thông số máu")]
        [SerializeField] private float _maxHealth = 100f; //Máu tối đa của người chơi
        [SerializeField] private float _currentHealth; //Máu hiện tại của người chơi

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _currentHealth = _maxHealth;
            _movement = GetComponent<PlayerMovement>();
        }

        //Hàm trừ máu, nếu damage là âm sẽ mất máu còn dương sẽ hồi máu
        public void ChangeHealth(float damage)
        {
            _currentHealth += damage;

            if(_currentHealth <= 0)
            {
                _currentHealth = 0;
                gameObject.SetActive(false);
                _movement._canMove = false;
            }
        }
    }
}

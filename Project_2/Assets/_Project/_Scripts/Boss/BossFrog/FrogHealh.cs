using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.Enemies.Bosses
{
    public class FrogHealth : MonoBehaviour, IDamageable
    {
        public float maxHealth = 200f;
        public float currentHealth;

        public GameObject healthUI;
        public Image healthBarFill;

        //private bool isStunned = false;

        private bool canTakeDamage = false; // <--- cờ kiểm soát nhận sát thương

        private void Start()
        {
            currentHealth = maxHealth;

            HideHealth();
            UpdateUI();
        }

        public void TakeDamage(float damage)
        {
            if (!canTakeDamage)
            {
                Debug.Log("Boss không nhận sát thương lúc nhảy hoặc idle");
                return;
            }
            else
            {
                float realDamage = -damage;
                currentHealth -= realDamage;

                if (currentHealth < 0)
                    currentHealth = 0;

                UpdateUI();

                // hiệu ứng bị đánh
                GetComponent<FrogController>().ChangeState(BossState.Hit);

                if (currentHealth <= 0)
                {
                    Die();
                }
            }
        }

        // Thêm phương thức để bật/tắt nhận sát thương từ bên ngoài (ví dụ từ FrogController)
        public void SetCanTakeDamage(bool value)
        {
            canTakeDamage = value;
        }

        //private void RemoveStun()
        //{
        //    isStunned = false;
        //    rb.linearVelocity = Vector2.zero;
        //}

        private void UpdateUI()
        {
            if (healthBarFill)
                healthBarFill.fillAmount = currentHealth / maxHealth;
        }

        public void HideHealth()
        {
            if (healthUI)
                healthUI.SetActive(false);
        }

        private void Die()
        {
            gameObject.SetActive(false);
        }

        public void ShowHealth()
        {
            if (healthUI)
                healthUI.SetActive(true);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _Project._Scripts.UI
{
    public class HUDController : MonoBehaviour
    {
        public static HUDController Instance { get; private set; }

        [Header("Giao diện HUD của player")]
        [SerializeField] private TMP_Text _healthText; //Text máu
        [SerializeField] private Image _healthImage; //Image máu

        [Space(10)]

        [SerializeField] private TMP_Text _energyText; //Text năng lượng
        [SerializeField] private Image _energyImage; //Image năng lượng

        [Space(10)]

        [SerializeField] private TMP_Text _staminaText; //Text thể lực
        [SerializeField] private Image _staminaImage; //Image thể lực

        [Header("Asset UI của game")]
        [Tooltip("Prefab để tạo Popup damage")]
        public Transform _pfDamagePopup; //Prefab tạo damage popup

        private void Awake()
        {
            //Tạo Singleton
            if(Instance != null) Destroy(gameObject);
            else Instance = this;
            //HUDController này chỉ có trong Game Scene nên không cần DontDestroyOnLoad
        }

        #region Health, Energy, Stamina của Player

        public void UpdateHealthUI(float currentHealth, float maxHealth)
        {
            if (_healthImage == null || _healthText == null) return;

            if(currentHealth >= maxHealth)
            {
                currentHealth = maxHealth;
            }
            else if (currentHealth <= 0f)
            {
                currentHealth = 0f;
            }

            _healthImage.fillAmount = currentHealth / maxHealth;
            _healthText.text = $"{currentHealth} / {maxHealth}";
        }

        public void UpdateEnergyUI(float currentEnergy, float maxEnergy)
        {
            if (_energyImage == null || _energyText == null) return;

            if(currentEnergy >= maxEnergy)
            {
                currentEnergy = maxEnergy;
            }
            else if(currentEnergy <= 0f)
            {
                currentEnergy = 0f;
            }

            _energyImage.fillAmount = currentEnergy / maxEnergy;
            _energyText.text = $"{currentEnergy} / {maxEnergy}";
        }

        public void UpdateStaminaUI(float currentStamina, float maxStamina)
        {
            if (_staminaImage == null || _staminaText == null) return;

            if(currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
            }
            else if (currentStamina <= 0f)
            {
                currentStamina = 0f;
            }

            _staminaImage.fillAmount = currentStamina / maxStamina;
            _staminaText.text = $"{currentStamina} / {maxStamina}";
        }

        #endregion
    }
}

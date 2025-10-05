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

            _staminaImage.fillAmount = currentStamina / maxStamina;
            _staminaText.text = $"{currentStamina} / {maxStamina}";
        }

        #endregion
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

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

        [Space(10)]

        [Header("Giao diện Item Popup")]
        [SerializeField] private GameObject _itemPopupContainer;
        [SerializeField] private GameObject _pfItemPopup;
        [Tooltip("Số lượng tối đa popup hiện trên UI")]
        [SerializeField] private int _maxPopup = 8;
        [SerializeField] private float _popupDuration;
        private readonly Queue<GameObject> _activePopups = new();

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

        #region Item Pickup Popup UI

        public void CreateItemPopup(string itemName, Sprite itemIcon)
        {
            GameObject newPopup = Instantiate(_pfItemPopup, _itemPopupContainer.transform);
            newPopup.GetComponentInChildren<TMP_Text>().text = itemName;

            Image imgIcon = newPopup.transform.Find("ItemIcon")?.GetComponent<Image>();
            if(imgIcon != null)
            {
                imgIcon.sprite = itemIcon;
            }

            _activePopups.Enqueue(newPopup);
            if(_activePopups.Count > _maxPopup)
            {
                Destroy(_activePopups.Dequeue());
            }

            StartCoroutine(ItemPopupFadeOut(newPopup));
        }

        IEnumerator ItemPopupFadeOut(GameObject popup)
        {
            yield return new WaitForSeconds(_popupDuration);
            if (popup == null) yield break;
            
            CanvasGroup popupCG = popup.GetComponent<CanvasGroup>();
            for(float t = 0f; t < 1f; t += Time.deltaTime)
            {
                if (popup == null) yield break;
                popupCG.alpha = 1f - t;
                yield return null;
            }

            Destroy(popup);
        }

        #endregion
    }
}

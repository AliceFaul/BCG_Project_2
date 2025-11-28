using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System;
using _Project._Scripts.Core;

namespace _Project._Scripts.UI
{
    public class HUDController : MonoBehaviour
    {
        public static HUDController Instance { get; private set; }
        CanvasGroup _uiCG;

        [Header("Giao diện HUD của player")]
        [SerializeField] private int _maxLevel;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private Image _experienceImage;
        [SerializeField] private TMP_Text _experienceText;
        [SerializeField] private GameObject _addExpButton;

        [Space(10)]

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

        [Header("Các property cần thiết cho Level System")]
        [SerializeField] private AnimationCurve _experienceCurve;
        private int _currentLevel, _totalExperience;
        private int _previousLevelsExperience, _nextLevelsExperience;
        public event Action OnLevelUp;

        public int CurrentLevel => _currentLevel;


        private void Awake()
        {
            //Tạo Singleton
            if(Instance != null) Destroy(gameObject);
            else Instance = this;
            //HUDController này chỉ có trong Game Scene nên không cần DontDestroyOnLoad

            _uiCG = GetComponent<CanvasGroup>();
            _currentLevel++;
            UpdateLevel();

            _addExpButton.GetComponent<Button>().onClick.AddListener(() => AddExperience(20));
        }

        #region Level UI Controller

        public void AddExperience(int amount)
        {
            if (_currentLevel >= _maxLevel) return;

            _totalExperience += amount;
            CheckForLevelUp();
            UpdateLevelInterface();
            Debug.LogWarning($"Add Experience to Player: {amount}");
        }

        void CheckForLevelUp()
        {
            bool isLevelUp = false;

            while(_totalExperience >= _nextLevelsExperience)
            {
                _currentLevel++;
                SoundEffectManager.Play("LevelUp");
                UpdateLevel();
                isLevelUp = true;
            }

            if(isLevelUp)
            {
                OnLevelUp?.Invoke();
                Debug.LogWarning("OnLevelUp Invoke");
            }
        }

        void UpdateLevel()
        {
            _previousLevelsExperience = (int)_experienceCurve.Evaluate(_currentLevel);
            _nextLevelsExperience = (int)_experienceCurve.Evaluate(_currentLevel + 1);
            UpdateLevelInterface();
        }

        void UpdateLevelInterface()
        {
            int start = _totalExperience - _previousLevelsExperience;
            int end = _nextLevelsExperience - _previousLevelsExperience;

            if(start < 0) start = 0;

            _levelText.text = _currentLevel.ToString();
            _experienceText.text = $"{start} / {end} exp";
            _experienceImage.fillAmount = (float)start / (float)end;
        }

        public PlayerLevelData GetLevelData()
        {
            PlayerLevelData levelData = new PlayerLevelData();

            levelData._currentLevel = _currentLevel;
            levelData._totalExperience = _totalExperience;

            return levelData;
        }

        public void SetPlayerLevelData(PlayerLevelData data)
        {
            _currentLevel = data._currentLevel;
            _totalExperience = data._totalExperience;

            UpdateLevel();
            UpdateLevelInterface();
        }

        #endregion

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

        public void HidePlayerHUD(bool isActive)
        {
            if (_uiCG == null) return;

            _uiCG.alpha = isActive ? 0f : 1f;
            _uiCG.interactable = !isActive;
            _uiCG.blocksRaycasts = !isActive;
        }
    }
}

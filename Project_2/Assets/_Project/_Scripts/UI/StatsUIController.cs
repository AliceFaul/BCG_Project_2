using _Project._Scripts.Core;
using _Project._Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.UI
{
    public class StatsUIController : MonoBehaviour
    {
        [Header("Text của các Stats")]
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private TMP_Text _energyText;
        [SerializeField] private TMP_Text _staminaText;
        [SerializeField] private TMP_Text _attackText;
        [SerializeField] private TMP_Text _defenseText;
        [SerializeField] private TMP_Text _skillDamageText;
        [SerializeField] private TMP_Text _pointsText;

        [Header("Button nâng cấp của các stats")]
        [SerializeField] private Button _healthButton;
        [SerializeField] private Button _energyButton;
        [SerializeField] private Button _staminaButton;
        [SerializeField] private Button _attackButton;
        [SerializeField] private Button _defenseButton;
        [SerializeField] private Button _skillDamageButton;

        PlayerStats _stats;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _stats = FindAnyObjectByType<PlayerStats>();

            _healthButton.onClick.AddListener(() => AddPoint("health"));
            _energyButton.onClick.AddListener(() => AddPoint("energy"));
            _staminaButton.onClick.AddListener(() => AddPoint("stamina"));
            _attackButton.onClick.AddListener(() => AddPoint("attack"));
            _defenseButton.onClick.AddListener(() => AddPoint("defense"));
            _skillDamageButton.onClick.AddListener(() => AddPoint("skilldamage"));

            _stats.OnPointChanged += UpdatePointText;
            _stats.OnStatChanged += UpdateStatsUI;

            UpdateStatsUI();
            UpdatePointText();
        }

        void AddPoint(string name)
        {
            _stats.IncreaseStats(name);
            UpdateStatsUI();
            SoundEffectManager.Play("PowerUp");
        }

        void UpdateStatsUI()
        {
            _healthText.text = $"Health: \t {_stats._healthPoint}";
            _energyText.text = $"Energy: \t {_stats._energyPoint}";
            _staminaText.text = $"Stamina: \t {_stats._staminaPoint}";
            _attackText.text = $"Attack: \t {_stats._attackPoint}";
            _defenseText.text = $"Defense: \t {_stats._defensePoint}";
            _skillDamageText.text = $"Skill DMG: \t {_stats._skillDamagePoint}";
        }

        void UpdatePointText()
        {
            _pointsText.text = $"Stat Points: {_stats.GetAvailablePoint}";
        }
    }
}

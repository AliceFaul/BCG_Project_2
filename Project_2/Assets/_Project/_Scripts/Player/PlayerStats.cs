using System;
using _Project._Scripts.Gameplay;
using UnityEngine;

namespace _Project._Scripts.Player
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("Các Stats căn bản của player trong game")] 
        public CharacterStat _Health = new CharacterStat(150);
        public CharacterStat _Energy = new CharacterStat(50);
        public CharacterStat _Stamina = new CharacterStat(100);
        public CharacterStat _Attack = new CharacterStat(10);
        public CharacterStat _Defense = new CharacterStat(5);
        public CharacterStat _SkillDamage = new CharacterStat(20);

        [Space(10)]

        public int _healthPoint = 1;
        public int _energyPoint = 1;
        public int _staminaPoint = 1;
        public int _attackPoint = 1;
        public int _defensePoint = 1;
        public int _skillDamagePoint = 1;

        [Space(10)]

        [Header("Stats point của Player")]
        private int _availablePoints;
        [SerializeField] private int _pointPerLevel = 3;

        public event Action OnStatChanged;
        public event Action OnPointChanged;
        public event Action OnStatsInitialize;

        #region Get all Stats Value

        public int Health => (int)_Health.Value;
        public int Energy => (int)_Energy.Value;
        public int Stamina => (int)_Stamina.Value;
        public int Defense => (int)_Defense.Value;
        public float Attack => (float)Math.Round(_Attack.Value, 4);
        public float SkillDamage => (float)Math.Round(_SkillDamage.Value);

        #endregion

        public int GetAvailablePoint => _availablePoints;

        private void Awake()
        {
            RecalculateStats();
        }

        public void OnLevelUp()
        {
            _availablePoints += _pointPerLevel;
            OnPointChanged?.Invoke();
        }

        #region Increase and Calculate Stats

        public void IncreaseStats(string name)
        {
            if (_availablePoints <= 0) return;

            switch(name)
            {
                case "health":
                    _healthPoint += 1;
                    break;
                case "energy":
                    _energyPoint += 1;
                    break;
                case "stamina":
                    _staminaPoint += 1;
                    break;
                case "attack":
                    _attackPoint += 1;
                    break;
                case "defense":
                    _defensePoint += 1;
                    break;
                case "skilldamage":
                    _skillDamagePoint += 1;
                    break;
            }

            _availablePoints--;
            OnPointChanged?.Invoke();
            OnStatChanged?.Invoke();

            RecalculateStats();
        }

        void RecalculateStats()
        {
            _Health._BaseValue = 150 + _healthPoint * 10;
            _Energy._BaseValue = 50 + _energyPoint * 15;
            _Stamina._BaseValue = 100 + _staminaPoint * 20;

            _Attack._BaseValue = 10 + Mathf.Log10(_attackPoint + 1) * 10f;
            _Defense._BaseValue = 5 + Mathf.Sqrt(_defensePoint) * 2f;
            _SkillDamage._BaseValue = 20 + Mathf.Log(_skillDamagePoint + 1) * 10f;

            MakeDirtyAll();
        }

        void MakeDirtyAll()
        {
            _Health._isDirty = true;
            _Energy._isDirty = true;
            _Stamina._isDirty = true;
            _Attack._isDirty = true;
            _Defense._isDirty = true;
            _SkillDamage._isDirty = true;
        }

        #endregion

        #region Stats Saving System

        public PlayerStatsData GetStatsData()
        {
            PlayerStatsData data = new PlayerStatsData();

            data._availablePts = _availablePoints;

            data._healthPts = _healthPoint;
            data._energyPts = _energyPoint;
            data._staminaPts = _staminaPoint;
            data._attackPts = _attackPoint;
            data._defensePts = _defensePoint;
            data._skillDamagePts = _skillDamagePoint;

            return data;
        }

        public void SetPlayerData(PlayerStatsData data)
        {
            if (data == null) return;

            _availablePoints = data._availablePts;

            _healthPoint = data._healthPts;
            _energyPoint = data._energyPts;
            _staminaPoint = data._staminaPts;
            _attackPoint = data._attackPts;
            _defensePoint = data._defensePts;
            _skillDamagePoint = data._skillDamagePts;

            RecalculateStats();

            OnStatChanged?.Invoke();
            OnPointChanged?.Invoke();
            OnStatsInitialize?.Invoke();
        }

        #endregion
    }
}

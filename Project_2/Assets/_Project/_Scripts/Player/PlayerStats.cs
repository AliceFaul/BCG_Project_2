using System;
using _Project._Scripts.Gameplay;
using UnityEngine;

namespace _Project._Scripts.Player
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("Các Stats của player trong game")] 
        public CharacterStat _Health = new CharacterStat(150);
        public CharacterStat _Energy = new CharacterStat(50);
        public CharacterStat _Stamina = new CharacterStat(100);
        public CharacterStat _Attack = new CharacterStat(10);
        public CharacterStat _Defense = new CharacterStat(5);
        public CharacterStat _SkillDamage = new CharacterStat(20);

        [Header("Stats point của Player")]
        public int _availablePoints;
        [SerializeField] private int _pointPerLevel = 3;

        public event Action OnStatChanged;
        public event Action OnPointChanged;

        public CharacterStat GetStatsByName(string name)
        {
            return name.ToLower()
            switch
            {
                "health" => _Health,
                "energy" => _Energy,
                "stamina" => _Stamina,
                "attack" => _Attack,
                "defense" => _Defense,
                "skilldamage" => _SkillDamage,
                _ => null
            };
        }

        public void OnLevelUp()
        {
            _availablePoints += _pointPerLevel;
            OnPointChanged?.Invoke();
        }

        public bool SpendStatPoint(string statName)
        {
            if(_availablePoints <= 0) return false;

            CharacterStat stat = GetStatsByName(statName);
            if(stat == null) return false;

            stat._BaseValue += 1;
            _availablePoints--;
            OnStatChanged?.Invoke();
            OnPointChanged?.Invoke();
            return true;
        }

        public int GetAvailablePoint() => _availablePoints;
    }
}

using System.Collections;
using System.Collections.Generic;
using _Project._Scripts.Gameplay;
using UnityEngine;

namespace _Project._Scripts.Player
{
    public class PlayerItemConsumer : MonoBehaviour
    {
        PlayerStats _stats;

        public float _consumableCooldown = 1f;
        bool _isConsumableCooldown = false;

        Dictionary<ItemSO, bool> _activeBuffs = new Dictionary<ItemSO, bool>();

        private void Awake()
        {
            _stats = GetComponent<PlayerStats>();
        }

        /// <summary>
        /// Hàm sử dụng item để buff hay áp dụng chỉ số lên Player
        /// </summary>
        /// <param name="item"></param>
        public bool UseItem(ItemSO item)
        {
            switch(item._type)
            {
                case ItemUseType.Consumable:
                    UseConsumeable(item);
                    return true;
                case ItemUseType.TemporaryBuff:
                    return TryUseTemporaryBuff(item);
                case ItemUseType.KeyItem:
                    Debug.Log("This is Key Item, can't use");
                    return true;
                case ItemUseType.PermanentStat:
                    foreach(var effect in item._effects)
                    {
                        ApplyPernament(effect, item);
                    }
                    return true;
            }

            return false;
        }

        // ==== Consumable Item ====
        void UseConsumeable(ItemSO item)
        {
            if(_isConsumableCooldown) return;

            foreach(var effect in item._effects)
            {
                switch(effect._statType)
                {
                    case StatType.Health:
                        GetComponent<PlayerHealth>().Heal((int)effect._value);
                        break;
                    case StatType.Stamina:
                        GetComponent<PlayerStamina>().ChangeStamina((int)effect._value);
                        break;
                }
            }

            StartCoroutine(ConsumableCooldownRoutine());
        }

        IEnumerator ConsumableCooldownRoutine()
        {
            _isConsumableCooldown = true;
            yield return new WaitForSeconds(_consumableCooldown);
            _isConsumableCooldown = false;
        }

        // ==== Temporary Item ====
        bool TryUseTemporaryBuff(ItemSO item)
        {
            if(_activeBuffs.ContainsKey(item) && _activeBuffs[item])
            {
                return false;
            }
            ApplyEffect(item);
            return true;
        }

        void ApplyEffect(ItemSO source)
        {
            _activeBuffs[source] = true;

            foreach (var effect in source._effects)
            {
                CharacterStat targetStat = GetsStat(effect._statType);

                if (targetStat == null) return;

                StatModifier modifier = new StatModifier(effect._value, effect._modifierType, source);

                targetStat.AddModifier(modifier);
                _stats.RefreshStats();

                StartCoroutine(RemoveAfterDelay(targetStat, modifier, effect._duration, source));
            }
        }

        void ApplyPernament(ItemEffect effect, ItemSO source)
        {
            if (_isConsumableCooldown) return;

            CharacterStat targetStat = GetsStat(effect._statType);
            if(targetStat == null) return;

            StatModifier modifier = new StatModifier(effect._value, effect._modifierType, source);
            targetStat.AddModifier(modifier);

            _stats.RefreshStats();

            StartCoroutine(ConsumableCooldownRoutine());
        }

        IEnumerator RemoveAfterDelay(CharacterStat stat, StatModifier modifier, float duration, ItemSO item)
        {
            yield return new WaitForSeconds(duration);
            stat.RemoveModifier(modifier);
            _activeBuffs[item] = false;
            _stats.RefreshStats();
        }

        /// <summary>
        /// Lấy stats dựa theo type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        CharacterStat GetsStat(StatType type)
        {
            return type switch
            {
                StatType.Stamina => _stats._Stamina,
                StatType.Attack => _stats._Attack,
                StatType.Health => _stats._Health,
                StatType.SkillDamage => _stats._SkillDamage,
                StatType.Defense => _stats._Defense,
                StatType.Energy => _stats._Energy,
                _ => null
            };
        }
    }
}

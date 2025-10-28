using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace _Project._Scripts.Gameplay
{
    [Serializable]
    public class CharacterStat
    {
        public float _BaseValue;

        protected readonly List<StatModifier> _statModifiers;
        public readonly ReadOnlyCollection<StatModifier> _StatModifiers;

        public virtual float Value
        {
            get
            {
                if (_isDirty || _lastBaseValue != _BaseValue)
                {
                    _lastBaseValue = _BaseValue;
                    _value = CalculateFinalValue();
                    _isDirty = false;
                }
                return _value;
            }
        }

        public bool _isDirty = true;
        protected float _value;
        protected float _lastBaseValue = float.MinValue;

        public CharacterStat()
        {
            _statModifiers = new List<StatModifier>();
            _StatModifiers = _statModifiers.AsReadOnly();
        }

        public CharacterStat(float baseValue) : this()
        {
            _BaseValue = baseValue;
        }

        public virtual void AddModifier(StatModifier modifier)
        {
            _isDirty = true;
            _statModifiers.Add(modifier);
            _statModifiers.Sort(CompareModifierOrder);
        }

        protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
        {
            if (a._Order < b._Order)
                return -1;
            else if (a._Order > b._Order)
                return 1;
            return 0;
        }

        public virtual bool RemoveModifier(StatModifier modifier)
        {
            if (_statModifiers.Remove(modifier))
            {
                _isDirty = true;
                return true;
            }
            return false;
        }

        public virtual bool RemoveAllModifiers(object source)
        {
            bool didRemove = false;

            for (int i = _statModifiers.Count - 1; i >= 0; i--)
            {
                if (_statModifiers[i]._Source == source)
                {
                    _isDirty = true;
                    didRemove = true;
                    _statModifiers.RemoveAt(i);
                }
            }

            return didRemove;
        }

        protected virtual float CalculateFinalValue()
        {
            float finalValue = _BaseValue;
            float sumPercentAdd = 0;

            for (int i = 0; i < _statModifiers.Count; i++)
            {
                StatModifier modifier = _statModifiers[i];

                if (modifier._Type == StatModifierType.Flat)
                {
                    finalValue += _statModifiers[i]._Value;
                }
                else if (modifier._Type == StatModifierType.PercentAdd)
                {
                    sumPercentAdd += modifier._Value;

                    if (i + 1 >= _statModifiers.Count ||
                        _statModifiers[i + 1]._Type != StatModifierType.PercentAdd)
                    {
                        finalValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0;
                    }
                }
                else if (modifier._Type == StatModifierType.PercentMult)
                {
                    finalValue *= 1 + modifier._Value;
                }
            }

            return (float)Math.Round(finalValue, 4);
        }
    }
}

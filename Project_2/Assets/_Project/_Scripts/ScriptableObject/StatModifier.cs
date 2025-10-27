namespace _Project._Scripts.Gameplay
{
    public class StatModifier
    {
        public readonly float _Value;
        public readonly StatModifierType _Type;
        public readonly int _Order;
        public readonly object _Source;

        public StatModifier(float value, StatModifierType type, int order, object source)
        {
            _Value = value;
            _Type = type;
            _Order = order;
            _Source = source;
        }

        public StatModifier(float value, StatModifierType type) : this(value, type, (int)type, null) { }
        public StatModifier(float value, StatModifierType type, int order) : this(value, type, order, null) { }
        public StatModifier(float value, StatModifierType type, object source) : this(value, type, (int)type, source) { }
    }

    public enum StatModifierType { Flat = 100, PercentAdd = 200, PercentMult = 300 }
}

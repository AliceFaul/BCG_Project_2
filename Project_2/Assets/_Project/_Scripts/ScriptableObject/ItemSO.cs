using _Project._Scripts.Gameplay;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class ItemSO : ScriptableObject
{
    [field :SerializeField]
    public int MaxStackSize { get; set; } = 1;
    [field: SerializeField]
    public string Name { get; set; }
    [field: SerializeField]
    [field: TextArea]
    public string Description { get; set; }
    [field: SerializeField]
    public Sprite ItemImage { get; set; }
    [field: SerializeField]
    public string Location { get; set; }

    [Header("In Shop Setting")]
    public int _price;
    public GameObject _itemPrefab;

    [Header("Phân loại và tác dụng của item")]
    public ItemUseType _type;
    public List<ItemEffect> _effects;
} 

public enum ItemUseType { Consumable, PermanentStat, TemporaryBuff, KeyItem, Equipment }

public enum StatType { Health, Energy, Stamina, Attack, Defense, SkillDamage }

/// <summary>
/// Hiệu ứng và tác dụng của item
/// </summary>
public struct ItemEffect
{
    public StatType _statType;
    public StatModifier _modifierType;
    public float _value;
    public float _duration;
}

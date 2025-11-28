using _Project._Scripts.Player;
using UnityEngine;

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
} 

public enum ItemUseType { Consumable, PermanentStat, TemporaryBuff, KeyItem, Equipment }

public enum StatType { Health, }

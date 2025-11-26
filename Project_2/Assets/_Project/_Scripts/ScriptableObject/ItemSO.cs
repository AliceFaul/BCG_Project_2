using _Project._Scripts.Gameplay;
using System.Globalization;
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

    public virtual void Use(StatModifier stats = null)
    {
        Debug.Log("Use Items");
    }
}

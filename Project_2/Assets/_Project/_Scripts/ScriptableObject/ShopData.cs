using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Script sẽ định dạng dữ liệu của một NPC shop
/// </summary>
/// 
[CreateAssetMenu(fileName = "NewShopData", menuName = "ShopData")]
public class ShopData : ScriptableObject
{
    public string _shopName;

    public List<ItemSO> _items;
}

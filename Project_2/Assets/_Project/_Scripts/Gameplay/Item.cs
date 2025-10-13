using _Project._Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// (Updated) Script này sẽ thiết lập thông số cho từng item khác nhau, dùng cho ItemDictionary và Save Item trong Inventory
/// </summary>
public class Item : MonoBehaviour
{
    public int _itemID; //ID của item
    public string _name; //Tên của item 

    public virtual void UseItem()
    {
        Debug.Log("Use Item " + _name); //Test bằng tạo log 
    }

    public virtual void Pickup()
    {
        Sprite itemIcon = GetComponent<Image>().sprite;
        if(HUDController.Instance == null)
        {
            return;
        }
        else
        {
            HUDController.Instance.CreateItemPopup(_name, itemIcon);
        }
    }
}

using _Project._Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// (Updated) Script này sẽ thiết lập thông số cho từng item khác nhau, dùng cho ItemDictionary và Save Item trong Inventory
/// </summary>
public class Item : MonoBehaviour
{
    public int _itemID; //ID của item
    public string _name; //Tên của item 

    public int _quantity = 1; //Số lượng của item
    [SerializeField] private TMP_Text _qtyText; //Text số lượng

    private void Start()
    {
        if(_qtyText == null)
        {
            _qtyText = GetComponentInChildren<TMP_Text>();
            Debug.Log("Implement qtyText");
        }
        UpdateQuantityDisplay();
    }

    #region Item Stack, Update Quantity

    //Hàm này giúp Update text số lượng của item bằng với _quantity(số lượng) trong script
    public void UpdateQuantityDisplay()
    {
        if (_qtyText == null)
        {
            Debug.LogWarning("Missing qty text");
            return;
        }

        //Nếu số lượng là 1 thì sẽ text để null, nghĩa là nếu số lượng từ 2 trở lên thì sẽ hiện text
        _qtyText.text = _quantity > 1 ? _quantity.ToString() : "";
    }

    //Hàm này sẽ sử dụng trong InventoryController
    //để tăng số lượng của item thay vì tạo item mới trong slot mới gọi là stack
    public void AddToStack(int amount = 1)
    {
        _quantity += amount;
        UpdateQuantityDisplay();
    }

    //Hàm này sử dụng trong việc drop item, sẽ trừ số lượng của item
    public int RemoveFromStack(int amount = 1)
    {
        int removedAmout = Mathf.Min(amount, _quantity);
        _quantity -= removedAmout;
        UpdateQuantityDisplay();
        return removedAmout;
    }

    //public GameObject CloneItem(int newQuantity)    
    //{
    //    GameObject clone = Instantiate(gameObject);
    //    Item cloneItem = clone.GetComponent<Item>();
    //    cloneItem._quantity = newQuantity;
    //    cloneItem.UpdateQuantityDisplay();
    //    return clone;
    //}

    #endregion

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

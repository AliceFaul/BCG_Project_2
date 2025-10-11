using UnityEngine;

/// <summary>
/// Script này để lưu item và sử dụng trong SaveData 
/// </summary>
[System.Serializable]
public class InventorySaveData
{
    public int _itemID; //ID của item trong Item Dictionary
    public int _slotIndex; //Vị trí của slot mà item đang nằm
}

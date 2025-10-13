using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public Vector3 _playerPosition; //Vị trí player trong game
    public string _mapBoundary; //Lấy theo tên của Boundary lưu
    public List<InventorySaveData> _inventorySaveData; //Data của item trong inventory page
    public List<InventorySaveData> _hotbarSaveData; //Data của item trong hotbar 
    public List<ChestSaveData> _chestSaveData; //Data của chest trong rương để lưu lại state của nó
}

[System.Serializable]
public class ChestSaveData
{
    public string _chestID;
    public bool _isOpened;
}

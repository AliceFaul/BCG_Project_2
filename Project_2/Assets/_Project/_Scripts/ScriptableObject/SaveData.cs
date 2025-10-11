using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public Vector3 _playerPosition; //Vị trí player trong game
    public string _mapBoundary; //Lấy theo tên của Boundary lưu
    public List<InventorySaveData> _inventorySaveData;
}

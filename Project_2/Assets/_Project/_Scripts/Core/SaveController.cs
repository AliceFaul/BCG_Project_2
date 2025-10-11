using UnityEngine;
using System.IO;
using Unity.Cinemachine;
using _Project._Scripts.UI;

namespace _Project._Scripts.Core
{
    public class SaveController : MonoBehaviour
    {
        private string _saveLocation;
        private InventoryController _invenController;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
            _invenController = FindAnyObjectByType<InventoryController>();

            LoadGame();
        }

        //Hàm dùng để save lại qua saveData và ghi vào Json
        public void SaveGame()
        {
            SaveData saveData = new SaveData
            {
                _playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
                _mapBoundary = FindAnyObjectByType<CinemachineConfiner2D>().BoundingShape2D.name,
                _inventorySaveData = _invenController.GetInventoryItems()
            };

            File.WriteAllText(_saveLocation, JsonUtility.ToJson(saveData));
        }

        //Hàm dùng để Load lại game qua Json
        public void LoadGame()
        {
            if(File.Exists(_saveLocation))
            {
                SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(_saveLocation));

                GameObject.FindGameObjectWithTag("Player").transform.position = saveData._playerPosition;
                FindAnyObjectByType<CinemachineConfiner2D>().BoundingShape2D = GameObject.Find(saveData._mapBoundary).GetComponent<PolygonCollider2D>();
                _invenController.SetInventoryItems(saveData._inventorySaveData);
            }
            //Nếu không tìm thấy file Json trong game thì sẽ tự động Save Game
            else
            {
                SaveGame();
            }
        }
    }
}

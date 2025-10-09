using UnityEngine;
using System.IO;

namespace _Project._Scripts.Core
{
    public class SaveController : MonoBehaviour
    {
        private string _saveLocation;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");

            LoadGame();
        }

        public void SaveGame()
        {
            SaveData saveData = new SaveData
            {
                _playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position
            };

            File.WriteAllText(_saveLocation, JsonUtility.ToJson(saveData));
        }

        public void LoadGame()
        {
            if(File.Exists(_saveLocation))
            {
                SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(_saveLocation));

                GameObject.FindGameObjectWithTag("Player").transform.position = saveData._playerPosition;
            }
            else
            {
                SaveGame();
            }
        }
    }
}

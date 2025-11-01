using UnityEngine;
using System.IO;
using Unity.Cinemachine;
using _Project._Scripts.UI;
using _Project._Scripts.Gameplay;
using System.Collections.Generic;
using System.Linq;
using _Project._Scripts.Player;

namespace _Project._Scripts.Core
{
    public class SaveController : MonoBehaviour
    {
        private string _saveLocation;
        private InventoryController _invenController;
        private HotbarController _hotbarController;
        private Chest[] _chests;
        private Checkpoint[] _checkpoints;
        private PlayerStats _playerStats;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Initialize();

            LoadGame();
        }

        //Hàm khởi tạo
        void Initialize()
        {
            _saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
            _invenController = FindAnyObjectByType<InventoryController>();
            _hotbarController = FindAnyObjectByType<HotbarController>();
            _chests = FindObjectsByType<Chest>(FindObjectsSortMode.None);
            _checkpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
            _playerStats = FindAnyObjectByType<PlayerStats>();
        }

        //Hàm dùng để save lại qua saveData và ghi vào Json
        public void SaveGame()
        {
            SaveData saveData = new SaveData
            {
                _playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
                _mapBoundary = FindAnyObjectByType<CinemachineConfiner2D>().BoundingShape2D.name,
                _inventorySaveData = _invenController.GetInventoryItems(),
                _hotbarSaveData = _hotbarController.GetHotBarItems(),
                _chestSaveData = GetChestState(),
                _questSaveData = QuestController.Instance._activeQuests,
                _handinQuestSaveData = QuestController.Instance._handinQuestIDs,
                _levelData = HUDController.Instance.GetLevelData(),
                _statsData = _playerStats.GetStatsData(),
                _checkpointSaveData = GetCheckpointState()
            };

            File.WriteAllText(_saveLocation, JsonUtility.ToJson(saveData));
        }

        #region Chest Saving System

        //Hàm này giúp lưu lại state của toàn bộ chest trong scene 
        List<ChestSaveData> GetChestState()
        {
            //Tạo một List chứa data state của toàn bộ chest trong scene
            List<ChestSaveData> chestStates = new List<ChestSaveData>();

            //Dùng foreach để lưu vào List
            foreach(Chest chest in _chests)
            {
                ChestSaveData chestsData = new ChestSaveData
                {
                    _chestID = chest._ChestID,
                    _isOpened = chest._IsOpened
                };
                chestStates.Add(chestsData);
            }

            return chestStates;
        }

        //Load chest theo state được lưu
        void LoadChestState(List<ChestSaveData> chestStates)
        {
            //Duyệt tất cả chest để load từng chest state
            foreach(Chest chest in _chests)
            {
                //Truy vấn chest bằng chest ID
                ChestSaveData chestSaveData = chestStates.FirstOrDefault(c => c._chestID == chest._ChestID);

                if(chestSaveData != null)
                {
                    chest.SetOpened(chestSaveData._isOpened);
                }
            }
        }

        #endregion

        #region Checkpoint Saving System

        List<CheckpointSaveData> GetCheckpointState()
        {
            List<CheckpointSaveData> checkpointStates = new List<CheckpointSaveData>();

            foreach(Checkpoint cp in _checkpoints)
            {
                CheckpointSaveData checkpointSaveData = new CheckpointSaveData
                {
                    _checkpointID = cp._CheckpointID,
                    _isActivated = cp._IsActivated
                };
                checkpointStates.Add(checkpointSaveData);
            }

            return checkpointStates;
        }

        void LoadCheckpointState(List<CheckpointSaveData> data)
        {
            if (data == null) return;

            foreach(Checkpoint cp in _checkpoints)
            {
                CheckpointSaveData checkpointSaveData = data.FirstOrDefault(c => c._checkpointID == cp._CheckpointID);

                if (checkpointSaveData != null)
                    cp.SetActivateCheckpoint(checkpointSaveData._isActivated);
            }
        }

        #endregion

        //Hàm dùng để Load lại game qua Json
        public void LoadGame()
        {
            if(File.Exists(_saveLocation))
            {
                SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(_saveLocation));

                GameObject.FindGameObjectWithTag("Player").transform.position = saveData._playerPosition;
                FindAnyObjectByType<CinemachineConfiner2D>().BoundingShape2D = GameObject.Find(saveData._mapBoundary).GetComponent<PolygonCollider2D>();
                _invenController.SetInventoryItems(saveData._inventorySaveData);
                _hotbarController.SetHotBarItems(saveData._hotbarSaveData);
                LoadChestState(saveData._chestSaveData);
                LoadCheckpointState(saveData._checkpointSaveData);
                QuestController.Instance.LoadQuestProgress(saveData._questSaveData);
                QuestController.Instance._handinQuestIDs = saveData._handinQuestSaveData;
                HUDController.Instance.SetPlayerLevelData(saveData._levelData);
                _playerStats.SetPlayerData(saveData._statsData);
            }
            //Nếu không tìm thấy file Json trong game thì sẽ tự động Save Game
            else
            {
                SaveGame();

                _invenController.SetInventoryItems(new List<InventorySaveData>());
                _hotbarController.SetHotBarItems(new List<InventorySaveData>());
            }
        }
    }
}

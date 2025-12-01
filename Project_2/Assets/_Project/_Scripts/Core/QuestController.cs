using UnityEngine;
using System.Collections.Generic;
using _Project._Scripts.UI;
using _Project._Scripts.Enemies;
using _Project._Scripts.Gameplay;

namespace _Project._Scripts.Core
{
    /// <summary>
    /// Script quản lý mọi thứ liên quan đến Quest
    /// </summary>
    public class QuestController : MonoBehaviour
    {
        //Singleton của QuestController để gọi qua các Script khác
        public static QuestController Instance { get; private set; }

        [Tooltip("Danh sách chứa các quest đang được người chơi thực hiện")]
        [SerializeField] public List<QuestProgress> _activeQuests = new();
        private QuestUIController _questUI;
        List<string> _seenQuests = new();
        public List<string> _handinQuestIDs = new();

        private void Awake()
        {
            //Thiết lập Singleton
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            _questUI = FindAnyObjectByType<QuestUIController>();
            InventoryController.Instance.OnInventoryChanged += CheckInventoryChanged;

            EnemyHealth.OnEnemyDefeated += CheckEnemyDefeated;

            NPC.OnNPCTalked += CheckNPCTalked;
        }

        /// <summary>
        /// Hàm nhận quest bằng cách thêm vào quest vào danh sách và cập nhật UI
        /// </summary>
        /// <param name="quest"></param>
        public void AcceptQuest(Quest quest)
        {
            //Nếu quest đã được nhận thì return
            if (IsQuestActive(quest._questID)) return;

            //Thêm quest vào danh sách và cập nhật UI
            _activeQuests.Add(new QuestProgress(quest));
            CheckInventoryChanged();
            _questUI.UpdateQuestLog();
        }

        public void LoadQuestProgress(List<QuestProgress> questSaveData)
        {
            _activeQuests = questSaveData ?? new();

            CheckInventoryChanged();
            _questUI.UpdateQuestLog();
        }

        /// <summary>
        /// Script có tác dụng trả quest lại khi đủ điều kiện (xóa quest khỏi quest log và nhận thưởng)
        /// </summary>
        /// <param name="questID"></param>
        public void HandinQuest(string questID)
        {
            //Kiểm tra nếu không đủ số lượng yêu cầu thì return
            if (!CheckRequiredItemFromInventory(questID)) return;

            //Đầu tiên là tìm quest bằng questID trong activeQuests
            //Sau đó xóa khỏi activeQuests đồng nghĩa là đã hoàn thành nhiệm vụ và sẽ không thể nhận lại
            //Lưu vào handinQuestIDs để quest này không thể nhận lại nữa
            QuestProgress quest = _activeQuests.Find(o => o.QuestID == questID);
            if (quest != null)
            {
                _handinQuestIDs.Add(questID);
                _activeQuests.Remove(quest);
                _questUI.UpdateQuestLog();
            }
        }

        #region Checking Quest State

        /// <summary>
        /// Hàm giúp để kiểm tra xem quest đã được nhận chưa (kiểm tra bằng QuestID)
        /// </summary>
        /// <param name="questID"></param>
        /// <returns></returns>
        public bool IsQuestActive(string questID) => _activeQuests.Exists(o => o.QuestID == questID); 

        /// <summary>
        /// Kiểm tra quest đã được hoàn thành chưa
        /// </summary>
        /// <param name="questID"></param>
        /// <returns></returns>
        public bool IsQuestCompleted(string questID)
        {
            QuestProgress quest = _activeQuests.Find(o => o.QuestID == questID);
            return quest != null && quest._objectives.TrueForAll(o => o.IsCompleted);
        }

        /// <summary>
        /// Kiểm tra quest đã được trả chưa
        /// </summary>
        /// <param name="questID"></param>
        /// <returns></returns>
        public bool IsQuestHandedIn(string questID) => _handinQuestIDs.Contains(questID);

        public bool HasSeenQuest(string questID)
        {
            return _seenQuests.Contains(questID);
        }

        public void MarkQuestSeen(string questID)
        {
            if (!_seenQuests.Contains(questID))
                _seenQuests.Add(questID);
        }

        public void MarkQuestUnseen(string questID)
        {
            _seenQuests.Remove(questID);
        }

        #endregion

        #region Quest Collect System

        /// <summary>
        /// Script này giúp cập nhật quest log nếu inventory có thay đổi (dựa vào event bên InventoryController)
        /// </summary>
        public void CheckInventoryChanged()
        {
            //Tạo 1 Dictionary lưu ID và số lượng của item trong inventory
            Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();

            //Duyệt qua trong activeQuests để cập nhật 
            foreach(QuestProgress quest in _activeQuests)
            {
                foreach(QuestObjective objective in quest._objectives)
                {
                    //Nếu type trong objective của quest không phải Collect thì bỏ qua
                    //Và nếu không cùng ID thì cũng bỏ qua
                    if(objective._type != ObjectiveType.Collect) continue;
                    if(!int.TryParse(objective._objectiveID, out int itemID)) continue;

                    //Lúc này thì sẽ thay đổi currentAmount của objective trong quest bằng giá trị mới
                    //Giá trị mới cũng là số lượng của item (lấy bằng itemID) trong inventory
                    int newAmount = itemCounts.TryGetValue(itemID, out int count) ? Mathf.Min(count, objective._requireAmount) : 0;

                    //Thay đổi currentAmount của objective bằng giá trị mới
                    if(objective._currentAmount != newAmount)
                    {
                        objective._currentAmount = newAmount;
                    }

                    if (quest.IsCompleted && !quest._completionPopupShown)
                    {
                        quest._completionPopupShown = true;

                        HUDController.Instance.QueueQuestPopup(
                            $"Quest Completed: {quest._quest._questName}"
                        );

                        HUDController.Instance.ShowPendingPopups();
                    }
                }
            }

            //Cập nhật Quest Log 
            _questUI.UpdateQuestLog();
        }

        /// <summary>
        /// Script kiểm tra item trong Inventory có đủ số lượng so với require của quest không
        /// và xóa item khỏi inventory khi đủ điều kiện hoàn thành
        /// </summary>
        /// <param name="questID"></param>
        /// <returns></returns>
        public bool CheckRequiredItemFromInventory(string questID)
        {
            //Đầu tiên là tìm quest trong các quest đang được nhận bằng questID
            QuestProgress quest = _activeQuests.Find(o => o.QuestID == questID);

            if(quest == null) return false;

            //Tạo 1 Dictionary để lưu itemID bằng objectiveID và số lượng yêu cầu hoàn thành
            Dictionary<int, int> requiredItems = new();

            //Duyệt qua các objective trong quest để lấy itemID và số lượng yêu cầu
            foreach(QuestObjective objective in quest._objectives)
            {
                //Nếu type của objective không phải Collect thì bỏ qua
                //Parse objectiveID qua thành itemID, lưu vào Dictionary để kiểm tra trong inventory
                if(objective._type == ObjectiveType.Collect &&
                    int.TryParse(objective._objectiveID, out int itemID))
                {
                    //Ví dụ: Item Meat có ID là 2 và cần thu thập 5 cái thì
                    //requiredItems[2] = 5; và InventoryController sẽ xóa 5 item có ID là 2
                    requiredItems[itemID] = objective._requireAmount;
                }
            }

            //Lúc này ta cần kiểm tra số lượng trong inventory có đủ không

            //Ta tạo một Dictionary để lưu số lượng item và ID của nó bằng GetItemCounts()
            Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();

            //Duyệt qua trong requiredItems để kiểm tra các item cần cho quest
            foreach(var item in requiredItems)
            {
                //Nếu số lượng trong itemCounts (Có nghĩa là số lượng trong inventory)
                //nhỏ hơn so với value của requiredItems (Nhắc lại: Value của requiredItems là số lượng của ID của item đó)
                //thì sẽ return false
                if (itemCounts.GetValueOrDefault(item.Key) < item.Value)
                    return false;
            }

            //Nếu các điều kiện trên không được đáp ứng có nghĩa là inventory của ta đủ số lượng với yêu cầu
            //và sẽ xóa item khỏi inventory bằng ID và số lượng sẽ xóa
            foreach(var item in requiredItems)
            {
                InventoryController.Instance.RemoveItemsFromInventory(item.Key, item.Value);
            }

            return true;
        }

        #endregion

        #region Quest Defeat System

        void CheckEnemyDefeated(string enemyID)
        {
            foreach(QuestProgress quest in _activeQuests)
            {
                foreach(QuestObjective objective in quest._objectives)
                {
                    if(objective._type != ObjectiveType.Defeat) continue;
                    if(objective._objectiveID != enemyID.ToString()) continue;

                    objective._currentAmount = Mathf.Min(objective._currentAmount + 1, objective._requireAmount);
                }

                if (quest.IsCompleted && !quest._completionPopupShown)
                {
                    quest._completionPopupShown = true;

                    HUDController.Instance.QueueQuestPopup(
                        $"Quest Completed: {quest._quest._questName}"
                    );

                    HUDController.Instance.ShowPendingPopups();
                }
            }
            _questUI.UpdateQuestLog();
        }

        #endregion

        #region Quest Talking System

        void CheckNPCTalked(string npcID)
        {
            foreach(QuestProgress quest in _activeQuests)
            {
                foreach(QuestObjective objective in quest._objectives)
                {
                    if(objective._type != ObjectiveType.Talk) continue;
                    if(objective._objectiveID != npcID) continue;

                    //Thường quest nói chuyện NPC chỉ nói với một người
                    objective._currentAmount = 1;
                }

                if(quest.IsCompleted && !quest._completionPopupShown)
                {
                    quest._completionPopupShown = true;

                    HUDController.Instance.QueueQuestPopup(
                        $"Quest Completed: {quest._quest._questName}"
                    );

                    HUDController.Instance.ShowPendingPopups();
                }
            }
            _questUI.UpdateQuestLog();
        }

        #endregion
    }
}

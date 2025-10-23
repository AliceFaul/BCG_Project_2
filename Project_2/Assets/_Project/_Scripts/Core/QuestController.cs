using UnityEngine;
using System.Collections.Generic;
using _Project._Scripts.UI;

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
        [SerializeField] private QuestUIController _questUI;

        private void Awake()
        {
            //Thiết lập Singleton
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            _questUI = FindAnyObjectByType<QuestUIController>();
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
            _questUI.UpdateQuestLog();
        }

        /// <summary>
        /// Hàm giúp để kiểm tra xem quest đã được nhận chưa (kiểm tra bằng QuestID)
        /// </summary>
        /// <param name="questID"></param>
        /// <returns></returns>
        public bool IsQuestActive(string questID) => _activeQuests.Exists(o => o.QuestID == questID); 
    }
}

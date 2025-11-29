using UnityEngine;
using TMPro;
using _Project._Scripts.Core;

namespace _Project._Scripts.UI
{
    public class QuestUIController : MonoBehaviour
    {
        [Header("Quest UI và Prefab")]
        [Tooltip("Là List chứa các prefab questEntry")]
        [SerializeField] private Transform _questListContent;

        [Space(10)]

        [Tooltip("Là prefab bảng quest chứa tên quest và objective")]
        [SerializeField] private GameObject _questEntryPrefab;
        [SerializeField] private GameObject _objectiveTextPrefab;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            UpdateQuestLog();
        }

        public void UpdateQuestLog()
        {
            if(_questListContent == null || _questListContent.Equals(null)) return;

            foreach (Transform child in _questListContent)
            {
                Destroy(child.gameObject);
            }

            foreach(var quest in QuestController.Instance._activeQuests)
            {
                GameObject questEntry = Instantiate(_questEntryPrefab, _questListContent);
                TMP_Text questNameText = questEntry.transform.Find("QuestNameText").GetComponent<TMP_Text>();
                Transform objectiveList = questEntry.transform.Find("ObjectList");

                questNameText.text = quest._quest._questName;

                foreach(var objective in quest._objectives)
                {
                    GameObject objTextGO = Instantiate(_objectiveTextPrefab, objectiveList);
                    TMP_Text objText = objTextGO.GetComponent<TMP_Text>();
                    objText.text = $"{objective._description} ({objective._currentAmount}/{objective._requireAmount})";
                }
            }
        }
    }
}

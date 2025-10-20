using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class QuestUIController : MonoBehaviour
{
    [Header("Quest UI và Prefab")]
    [Tooltip("Là List chứa các prefab questEntry")]
    [SerializeField] private Transform _questListContent;

    [Space(10)]

    [Tooltip("Là prefab bảng quest chứa tên quest và objective")]
    [SerializeField] private GameObject _questEntryPrefab;
    [SerializeField] private GameObject _objectiveTextPrefab;

    //Các biến dữ liệu quan trọng
    [SerializeField] private Quest _quest; //Sử dụng để thay đổi questName và objective trong prefab questEntry
    [SerializeField] private int _questAmount;
    private List<QuestProgress> _questProgress = new(); //Sẽ sử dụng để chứa các quest đang thực hiện

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < _questAmount; i++)
        {
            _questProgress.Add(new QuestProgress(_quest));
        }

        UpdateQuestLog();
    }

    public void UpdateQuestLog()
    {
        foreach(Transform child in _questListContent)
        {
            Destroy(child.gameObject);
        }

        foreach(var quest in _questProgress)
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

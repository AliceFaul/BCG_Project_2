using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// ScriptableObject lưu lại dữ liệu nhiệm vụ
/// </summary>
[CreateAssetMenu(menuName = "Quests/QuestData", fileName = "NewQuestData")]
public class Quest : ScriptableObject
{
    public string _questID;
    public string _questName;
    public string _description;
    public List<QuestObjective> _objectives;

    private void OnValidate()
    {
        if(string.IsNullOrEmpty(_questID))
        {
            _questID = _questName + Guid.NewGuid().ToString();
        }
    }
}

[System.Serializable]
public class QuestObjective
{
    public string _objectiveID;
    public string _description;
    public ObjectiveType _type;
    public int _requireAmount;
    public int _currentAmount;

    public bool IsCompleted => _currentAmount >= _requireAmount;
}

public enum ObjectiveType { Collect, Defeat, ReachLocation, Talk, Custom }

[System.Serializable]
public class QuestProgress
{
    public Quest _quest;
    public List<QuestObjective> _objectives;

    public QuestProgress(Quest quest)
    {
        _quest = quest;
        _objectives = new List<QuestObjective>();

        foreach(var obj in _quest._objectives)
        {
            _objectives.Add(new QuestObjective
            {
                _objectiveID = obj._objectiveID,
                _description = obj._description,
                _type = obj._type,
                _requireAmount = obj._requireAmount,
                _currentAmount = 0
            });
        }
    }

    public bool IsCompleted => _objectives.TrueForAll(x => x.IsCompleted);
    public string QuestID => _quest._questID;
}


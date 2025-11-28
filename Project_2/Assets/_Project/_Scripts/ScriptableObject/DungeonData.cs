using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDungeonData", menuName = "DungeonData")]
public class DungeonData : ScriptableObject
{
    [Header("Thông tin của Dungeon")]
    public string _dungeonID;
    public string _dungeonName;
    public float _resetTime;
    public bool _isCleared;

    public int _normalLevelRequirement;
    public int _hardLevelRequirement;
    public Difficult _dungeonDifficult;

    [HideInInspector] public float _lastClearTime;

#if UNITY_EDITOR
    private void OnValidate()
    {
        _dungeonID = _dungeonName + Guid.NewGuid().ToString();

        if (_normalLevelRequirement <= 0) _normalLevelRequirement = 1;
        else if(_hardLevelRequirement <= _normalLevelRequirement) _hardLevelRequirement = _normalLevelRequirement + 5;
    }
#endif

    public int GetLevelRequirement(Difficult diff)
    {
        return diff switch
        {
            Difficult.Normal => _normalLevelRequirement,
            Difficult.Hard => _hardLevelRequirement,
            _ => 1
        };
    }

    public bool IsOnCooldown() => Time.time < _lastClearTime;
}

public enum Difficult { Normal, Hard }

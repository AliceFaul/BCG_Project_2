using UnityEngine;
using System;

[System.Serializable]
public class LevelData
{
    public event EventHandler OnExperienceChanged;
    public event EventHandler OnLevelChanged;

    private int _level;
    private int _experience;
    private int _experienceToNextLevel;

    public LevelData()
    {
        _level = 0;
        _experience = 0;
        _experienceToNextLevel = 100;
    }

    public void AddExperience(int amount)
    {
        _experience += amount;

        if (_experience >= _experienceToNextLevel)
        {
            _level++;
            _experience -= _experienceToNextLevel;

            if(OnLevelChanged != null)
                OnLevelChanged(this, EventArgs.Empty);
        }

        if(OnExperienceChanged != null)
            OnExperienceChanged(this, EventArgs.Empty);
    }

    public int GetLevel() => _level;

    public float GetExperience() => (float)_experience / _experienceToNextLevel;
}


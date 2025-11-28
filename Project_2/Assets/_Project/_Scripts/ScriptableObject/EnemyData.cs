using UnityEngine;

/// <summary>
/// Dữ liệu của enemy chứa các thông tin cơ bản
/// </summary>
[CreateAssetMenu(fileName = "NewEnemyData", menuName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Các thông số cơ bản của một enemy")]
    //Tên và ID
    public string _enemyID;
    public string _enemyName;
    //Các chỉ số máu và tấn công
    public float _baseHealth;
    public float _baseDamage;
    public int _baseExperience;

    [Header("Lượng scale up stat khi người chơi lên level")]
    public float _healthGrowth = 1.15f;
    public float _damageGrowth = 1.10f;
    public float _experienceGrowth = 1.10f;

    public EnemyStats GetStatsAfterGrowth(int playerLevel)
    {
        EnemyStats stats = new EnemyStats();

        stats._enemyHP = Mathf.RoundToInt(_baseHealth * Mathf.Pow(_healthGrowth, playerLevel));
        stats._enemyDamage = Mathf.RoundToInt(_baseDamage * Mathf.Pow(_damageGrowth, playerLevel));
        stats._enemyExperience = Mathf.RoundToInt(_baseExperience * Mathf.Pow(_experienceGrowth, playerLevel));

        return stats;
    }
}

[System.Serializable]
public class EnemyStats
{
    public int _enemyHP;
    public int _enemyDamage;
    public int _enemyExperience;
}

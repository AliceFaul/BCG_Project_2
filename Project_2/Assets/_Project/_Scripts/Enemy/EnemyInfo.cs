using UnityEngine;

namespace _Project._Scripts.Enemies
{
    public class EnemyInfo : MonoBehaviour
    {
        public EnemyData _enemyData;

        public string GetEnemyID => _enemyData._enemyID;
        public string GetEnemyName => _enemyData._enemyName;
    }
}

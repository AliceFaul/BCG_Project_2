using System.Collections;
using System.Linq;
using _Project._Scripts.Gameplay;
using _Project._Scripts.UI;
using Unity.Cinemachine;
using UnityEngine;

namespace _Project._Scripts.Core
{
    public class DungeonController : MonoBehaviour
    {   
        public static DungeonController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void EnterDungeon(DungeonData data, PolygonCollider2D dungeonBoundary, Vector3 entryPoint, Difficult difficult)
        {
            StartCoroutine(FadeTransition.Instance.FadeOutIn(() =>
            {
                HUDController.Instance?.HidePlayerHUD(false);
                GameObject player = GameObject.FindWithTag("Player");
                if (player == null) return;

                player.transform.position = entryPoint;
                FindAnyObjectByType<CinemachineConfiner2D>().BoundingShape2D = dungeonBoundary;

                int enemyLevel = data.GetLevelRequirement(difficult);

                ApplyDifficult(enemyLevel, dungeonBoundary);
            }));
        }

        public void ExitDungeon(DungeonData data, PolygonCollider2D overworldBoundary, Vector3 exitPoint)
        {
            StartCoroutine(FadeTransition.Instance.FadeOutIn(() =>
            {
                HUDController.Instance?.HidePlayerHUD(false);
                GameObject player = GameObject.FindWithTag("Player");
                if (player == null) return;

                player.transform.position = exitPoint;
                FindAnyObjectByType<CinemachineConfiner2D>().BoundingShape2D = overworldBoundary;
            }));
            
            if(data._isCleared)
            {
                StartCoroutine(StartCooldown(data));
            }
        }

        IEnumerator StartCooldown(DungeonData data)
        {
            data._lastClearTime = Time.time + data._resetTime;
            Debug.Log($"{data._dungeonName} in cooldown {data._resetTime} seconds");
            yield return new WaitForSeconds(data._resetTime);
            data._isCleared = false;
            Debug.Log($"{data._dungeonName} has reseted");
        }

        public void ClearDungeon(DungeonData data)
        {
            data._isCleared = true;
            Debug.Log($"{data._dungeonName} has cleared");
        }

        #region Apply Diffcult into Dungeon

        void ApplyDifficult(int level, PolygonCollider2D boundary)
        {
            Debug.Log($"Dungeon started with diffult have level {level}");

            if (boundary == null)
                return;

            IDungeonEnemy[] dungeonEnemies = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDungeonEnemy>().ToArray();

            foreach(var e in dungeonEnemies)
            {
                var mb = e as MonoBehaviour;
                if(mb == null) continue;

                Vector2 pos = mb.transform.position;
                if(boundary.OverlapPoint(pos))
                {
                    var enemyDataHolder = mb.GetComponent<IEnemyDataHolder>();
                    if(enemyDataHolder == null)
                        continue;

                    var enemyData = enemyDataHolder.GetEnemyData();
                    if(enemyData == null) continue;

                    EnemyStats stats = enemyData.GetStatsAfterGrowth(level);
                    e.ApplyStats(stats);
                }
            }
        }

        #endregion

        #region Dungeon chest Setting

        /// <summary>
        /// Script gọi trong Chest để chest xác nhận người chơi đã hoàn thành Dungeon
        /// </summary>
        /// <param name="chest"></param>
        /// <param name="boundary"></param>
        /// <param name="data"></param>
        public void OnChestOpened(Chest chest, PolygonCollider2D boundary, DungeonData data)
        {
            StartCoroutine(HandleChestRoutine(chest, boundary, data));
        }

        IEnumerator HandleChestRoutine(Chest chest, PolygonCollider2D boundary, DungeonData data)
        {
            IDungeonEnemy[] dungeonEnemies = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDungeonEnemy>().ToArray();

            foreach(var e in dungeonEnemies)
            {
                var mb = e as MonoBehaviour;
                if(mb == null) continue;

                if(boundary != null && !boundary.OverlapPoint(mb.transform.position)) continue;

                e.ResetForDungeon();
            }

            if (data != null) ClearDungeon(data);

            yield return new WaitForSeconds(5f);

            var exit = FindObjectsByType<DungeonEntry>(FindObjectsSortMode.None).Where(e => e._type == PortalType.Exit);
            if(exit != null && exit.Any())
            {
                DungeonEntry selectedExit = null;
                foreach(var e in exit)
                {
                    if (e._overworldBoundary == null) continue;

                    selectedExit = e;
                    break;
                }

                if(selectedExit != null)
                {
                    if(data != null)
                        ExitDungeon(data, selectedExit._overworldBoundary, 
                            selectedExit._exitPoint.position);
                }
            }
        }

        #endregion
    }
}

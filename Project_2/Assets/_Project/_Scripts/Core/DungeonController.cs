using System.Collections;
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

        public void EnterDungeon(DungeonData data, PolygonCollider2D dungeonBoundary, Vector3 entryPoint)
        {
            StartCoroutine(FadeTransition.Instance.FadeOutIn(() =>
            {
                HUDController.Instance?.HidePlayerHUD(false);
                GameObject player = GameObject.FindWithTag("Player");
                if (player == null) return;

                player.transform.position = entryPoint;
                FindAnyObjectByType<CinemachineConfiner2D>().BoundingShape2D = dungeonBoundary;
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
    }
}

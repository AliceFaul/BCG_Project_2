using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _Project._Scripts.Enemies;

public class EnemySpawner : MonoBehaviour
{
    [Header("Pool & Spawn Settings")]
    [SerializeField] private EnemyPool enemyPool;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Wave Settings")]
    [SerializeField] private int enemiesPerWave = 3;   // M?i ð?t spawn bao nhiêu con
    [SerializeField] private float timeBetweenWaves = 2f;  // Th?i gian ngh? gi?a các ð?t
    [SerializeField] private int totalEnemies = 10;   // T?ng s? enemy c?n spawn

    private bool isSpawning = false;
    private int totalSpawned = 0;
    private int currentWave = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            totalSpawned = 0;
            currentWave = 0;
            StartCoroutine(WaveRoutine());
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    private IEnumerator WaveRoutine()
    {
        while (isSpawning && totalSpawned < totalEnemies)
        {
            currentWave++;
            Debug.Log($" Wave {currentWave} started");

            // Spawn 1 ð?t enemy
            SpawnWave();

            // Ð?i t?t c? enemy trong ð?t này ch?t
            yield return new WaitUntil(() => activeEnemies.Count == 0);

            Debug.Log($" Wave {currentWave} cleared");

            // Ngh? 1 chút trý?c khi spawn ð?t m?i (n?u chýa ð? t?ng s?)
            if (totalSpawned < totalEnemies)
                yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log(" All enemies spawned and cleared!");
        isSpawning = false;
    }

    private void SpawnWave()
    {
        int remaining = totalEnemies - totalSpawned; // c?n bao nhiêu con chýa spawn
        int count = Mathf.Min(enemiesPerWave, remaining);

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject enemyObj = enemyPool.GetFromPool();
            enemyObj.transform.position = spawnPoint.position;
            enemyObj.SetActive(true);

            totalSpawned++;
            activeEnemies.Add(enemyObj);

            EnemyHealth eh = enemyObj.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.OnDead += () =>
                {
                    activeEnemies.Remove(enemyObj);
                };
            }
        }

        Debug.Log($"Spawned {count} enemies (total {totalSpawned}/{totalEnemies})");
    }
}

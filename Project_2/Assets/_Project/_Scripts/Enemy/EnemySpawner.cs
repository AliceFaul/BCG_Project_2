using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _Project._Scripts.Enemies;
using System;
using _Project._Scripts.Core;

public class EnemySpawner : MonoBehaviour
{
    public event Action ClearSpawner;
    Transform _player;

    [Header("Pool & Spawn Settings")]
    [SerializeField] private EnemyPool enemyPool;
    [SerializeField] private float _minDistance; 
    [SerializeField] private float _maxDistance; 
    [SerializeField] private float _exitDistance; 

    [Header("Wave Settings")]
    [SerializeField] private int enemiesPerWave = 3;   // M?i ð?t spawn bao nhiêu con
    [SerializeField] private float timeBetweenWaves = 2f;  // Th?i gian ngh? gi?a các ð?t
    [SerializeField] private int totalEnemies = 10;   // T?ng s? enemy c?n spawn

    private bool isSpawning = false;
    private int totalSpawned = 0;
    private int currentWave = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Update()
    {
        if (!isSpawning || _player == null) return;

        if(Vector2.Distance(transform.position, _player.position) > _exitDistance)
        {
            StopSpawning();
            BGMController.Instance.ChangeMusicMode(MusicMode.Normal);
        }
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            totalSpawned = 0;
            currentWave = 0;

            if(_player == null)
                _player = GameObject.FindWithTag("Player").transform;

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
        BGMController.Instance?.ChangeMusicMode(MusicMode.Battle);

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

        BGMController.Instance?.ChangeMusicMode(MusicMode.Normal);
        ClearSpawner?.Invoke();
        isSpawning = false;
    }

    private void SpawnWave()
    {
        int remaining = totalEnemies - totalSpawned; // c?n bao nhiêu con chýa spawn
        int count = Mathf.Min(enemiesPerWave, remaining);

        for (int i = 0; i < count; i++)
        {
            Vector2 spawnOffset = UnityEngine.Random.insideUnitCircle.normalized *
            UnityEngine.Random.Range(_minDistance, _maxDistance);
            Vector2 spawnPosition = (Vector2)transform.position + spawnOffset;

            GameObject enemyObj = enemyPool.GetFromPool();
            enemyObj.transform.position = spawnPosition;
            enemyObj.transform.rotation = Quaternion.identity;

            EnemyHealth eh = enemyObj.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.OnDead -= () => OnEnemiesDead(enemyObj);
                eh.OnDead += () => OnEnemiesDead(enemyObj);
            }

            enemyObj.SetActive(true);
            totalSpawned++;
            activeEnemies.Add(enemyObj);
        }

        Debug.Log($"Spawned {count} enemies (total {totalSpawned}/{totalEnemies})");
    }

    void OnEnemiesDead(GameObject enemyObj)
    {
        activeEnemies.Remove(enemyObj);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _minDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _maxDistance);
    }
}

using _Project._Scripts.Enemies;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private bool expandIfEmpty = true;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private EnemyHealth EnemyHealth;
    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(enemyPrefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetFromPool()
    {
        GameObject obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            if (expandIfEmpty)
            {
                obj = Instantiate(enemyPrefab, transform);
            }
            else
            {
                return null;
            }
        }

        obj.SetActive(true);

        EnemyHealth eh = obj.GetComponent<EnemyHealth>();
        if (eh != null) eh.Initialize(this);

        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform); 
        pool.Enqueue(obj);
    }
}

using System.Collections;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private float _resetTime;

    private bool hasTriggered = false;

    private void Start()
    {
        if(spawner != null)
        {
            spawner.ClearSpawner += () => StartCoroutine(ResetCountdown(_resetTime));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered && triggerOnce) return;
        if (other.CompareTag("Player"))
        {
            spawner?.StartSpawning();
            if (triggerOnce) hasTriggered = true;
        }
    }

    IEnumerator ResetCountdown(float resetTime)
    {
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(resetTime);

        GetComponent<Collider2D>().enabled = true;
        hasTriggered = false;
    }

    //private void OnTriggerExit2D(Collider2D other)
    //{
        
      //   if (other.CompareTag("Player")) spawner?.StopSpawning();
   // }
}

using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered && triggerOnce) return;
        if (other.CompareTag("Player"))
        {
            spawner?.StartSpawning();
            if (triggerOnce) hasTriggered = true;
        }
    }

    //private void OnTriggerExit2D(Collider2D other)
    //{
        
      //   if (other.CompareTag("Player")) spawner?.StopSpawning();
   // }
}

using _Project._Scripts.Player;
using UnityEngine;

public class Skill3Zone : MonoBehaviour
{
    public float duration = 5f; // thời gian tồn tại của vùng
    public float freezeDuration = 5f; // player bị đứng yên 5s

    private void Start()
    {
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("cham vao player");
            PlayerMovement pm = col.GetComponent<PlayerMovement>();

            if (pm != null)
                pm.Freeze(freezeDuration);
        }
    }
}

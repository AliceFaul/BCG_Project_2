using System.Collections;
using UnityEngine;

public class FrogController : MonoBehaviour
{
    public Animator anim;
    public Transform player;
    private bool isCharging = false;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float enragedMoveSpeed = 6f; // tốc độ khi giận dữ
    private float currentSpeed;

    [Header("Attack Spawn Enemy")]
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;
    private float spawnTimer;

    [Header("Damage To Player")]
    public float touchDamage = 20f;

    [Header("Enrage (<=50% máu)")]
    public bool hasEnraged = false;
    public string chargeAnim = "Chare";

    private bool isActive = false;
    private bool isReturning = false;

    private Vector2 startPos;

    private FrogHealth health;

    private void Start()
    {
        startPos = transform.position;
        currentSpeed = moveSpeed;
        health = GetComponent<FrogHealth>();
    }

    private void Update()
    {
        if (!isActive)
        {
            if (isReturning)
                ReturnToStart();
            return;
        }

        // boss đuổi player bằng Jump animation
        anim.SetBool("JumpAttact", true);
        FollowPlayer();

        // spawn enemy theo chu kỳ
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0;
            anim.SetTrigger("Attact");
            Invoke(nameof(SpawnEnemy), 0.2f);
        }

        // nếu máu <= 50% → chạy charge 1 lần
        CheckEnrage();
    }

    private void FollowPlayer()
    {
        if (player == null || isCharging) return; // đang charge thì đứng yên

        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * currentSpeed * Time.deltaTime);
    }


    private void SpawnEnemy()
    {
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }

    public void OnPlayerEnter()
    {
        isActive = true;
        isReturning = false;
        anim.SetBool("JumpAttact", true);
    }

    public void OnPlayerExit()
    {
        isActive = false;
        isReturning = true;
        anim.SetBool("JumpAttact", false);
    }

    private void ReturnToStart()
    {
        transform.position = Vector2.MoveTowards(transform.position, startPos, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, startPos) < 0.1f)
        {
            isReturning = false;
        }
    }

    private void CheckEnrage()
    {
        if (!hasEnraged && health.currentHealth <= health.maxHealth * 0.5f)
        {
            hasEnraged = true;
            StartCoroutine(DoChargeAnimation());
        }
    }

    private IEnumerator DoChargeAnimation()
    {
        isCharging = true;               // đứng yên, không đuổi player
        anim.SetBool("JumpAttact", false);
        anim.SetTrigger("Chare");

        currentSpeed = 0f;               // tốc độ = 0 để đứng yên

        yield return new WaitForSeconds(10f); // thời gian charge

        currentSpeed = enragedMoveSpeed; // tăng tốc độ khi hết charge
        isCharging = false;              // tiếp tục đuổi player
        anim.SetBool("JumpAttact", true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IDamageable dmg = collision.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(touchDamage);
            }
        }
    }

}

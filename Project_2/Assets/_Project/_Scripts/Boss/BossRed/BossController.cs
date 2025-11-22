using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Boss Logic")]
    public bool isActive = false;
    public bool hasStarted = false;
    public Animator anim;
    public Rigidbody2D rb;

    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;
    private Transform targetPoint;

    [Header("Move Settings")]
    public float speed = 2f;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;    // Prefab viên đạn
    public float fireDelay = 3f;       // 3 giây mỗi lần bắn
    private bool canShoot = false;     // kiểm soát bắn khi player ở trong vùng

    [Header("Pillars")]
    public BossPillar pillarA;
    public BossPillar pillarB;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        rb.simulated = false;
        anim.enabled = false;

        targetPoint = pointA;
    }

    private void Update()
    {
        if (!isActive) return;
        Patrol();
    }

    // ---------------------------------------------------------
    // Player bước vào vùng → kích hoạt boss
    // ---------------------------------------------------------
    public void ActivateBoss()
    {
        if (!hasStarted)
        {
            anim.enabled = true;
            rb.simulated = true;
            anim.SetTrigger("StartFight");

            hasStarted = true;

            Invoke(nameof(StartPatrol), 1.2f);
            InvokeRepeating(nameof(Shoot4Directions), 1.2f, fireDelay);
            canShoot = true;
        }
        else
        {
            StartPatrol();
            canShoot = true;
            InvokeRepeating(nameof(Shoot4Directions), 0f, fireDelay);
        }
    }

    void StartPatrol()
    {
        isActive = true;
    }

    // ---------------------------------------------------------
    // Player rời khỏi vùng → DỪNG bắn + dừng di chuyển
    // ---------------------------------------------------------
    public void StopBoss()
    {
        isActive = false;
        rb.linearVelocity = Vector2.zero;

        canShoot = false;
        CancelInvoke(nameof(Shoot4Directions));
    }

    // ---------------------------------------------------------
    // Patrol A ↔ B
    // ---------------------------------------------------------
    void Patrol()
    {
        if (targetPoint == null)
        {
            Debug.LogWarning("targetPoint đã bị phá!");
            return;
        }

        Vector2 dir = (targetPoint.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
        }
    }


    // ---------------------------------------------------------
    // BẮN ĐẠN 4 HƯỚNG
    // ---------------------------------------------------------
    void Shoot4Directions()
    {
        if (!canShoot) return;

        // CHẠY ANIMATION ATTACK
        anim.SetTrigger("isAttack");

        // Bắn 4 hướng cơ bản
        SpawnBullet(Vector2.up);
        SpawnBullet(Vector2.down);
        SpawnBullet(Vector2.left);
        SpawnBullet(Vector2.right);

        // Nếu cả 2 cột đã phá → bắn thêm 4 hướng chéo
        if (pillarA.IsBroken && pillarB.IsBroken)
        {
            SpawnBullet(new Vector2(1, 1).normalized);   // lên phải
            SpawnBullet(new Vector2(-1, 1).normalized);  // lên trái
            SpawnBullet(new Vector2(1, -1).normalized);  // xuống phải
            SpawnBullet(new Vector2(-1, -1).normalized); // xuống trái
        }
    }


    void SpawnBullet(Vector2 dir)
    {
        GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        b.GetComponent<Bullet>().direction = dir;
    }
}

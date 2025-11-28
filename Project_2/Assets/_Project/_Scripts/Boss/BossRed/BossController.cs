using UnityEngine;

namespace _Project._Scripts.Enemies.Bosses
{
    public class BossController : MonoBehaviour
    {
        BossHealth health;

        [Header("Boss Logic")]
        public bool isActive = false;
        public bool hasStarted = false;
        public Animator anim;
        public Rigidbody2D rb;

        [Header("Patrol Points")]
        public Transform pointA;
        public Transform pointB;
        private Transform targetPoint;

        [Header("Attack Setting")]
        float _attackTimer;
        [SerializeField] private float _attackCD = 3f;

        [Header("Move Settings")]
        public float speed = 2f;
        public float brokenPillarsSpeed = 4f;     // tốc độ khi cả 2 trụ phá

        [Header("Bullet Settings")]
        public GameObject bulletPrefab;    // Prefab viên đạn
        public float fireDelay = 5f;       // 3 giây mỗi lần bắn
        private bool canShoot = false;     // kiểm soát bắn khi player ở trong vùng

        [Header("Pillars")]
        public BossPillar pillarA;
        public BossPillar pillarB;

        [SerializeField] BossRedState _state;

        [Header("Random Patrol Settings")]
        public CircleCollider2D triggerZone; // vùng trigger của boss
        public float randomPatrolSpeed = 2f;
        public float reachThreshold = 0.2f; // khoảng cách để boss tính là tới điểm
        private Vector2 currentTarget;       // điểm hiện tại boss đang đi
        private bool useRandomPatrol = false;

        private void Start()
        {
            anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<BossHealth>();

            rb.simulated = false;
            anim.enabled = false;

            targetPoint = pointA;

            _attackTimer = _attackCD;
        }

        private void OnEnable()
        {
            if (health != null)
            {
                health.OnBossDead += ResetBoss;
                Debug.Log("Boss Red has subscribe revive");
            }
        }

        private void OnDisable()
        {
            if (health != null)
            {
                health.OnBossDead -= ResetBoss;
                Debug.Log("Boss Red has subscribe revive");
            }
        }

        private void Update()
        {
            if (!isActive) return;

            // Nếu boss đang random patrol → chỉ di chuyển random
            if (useRandomPatrol)
            {
                RandomPatrolUpdate();
            }
            else
            {
                Patrol();
            }
            if (_attackTimer > 0)
            {
                _attackTimer -= Time.deltaTime;
            }
            else
            {
                ChangeState(BossRedState.Attack);
                _attackTimer = _attackCD;
            }

            // Kiểm tra nếu cả 2 trụ bị phá → bật random patrol
            if (pillarA.IsBroken && pillarB.IsBroken && !useRandomPatrol)
            {
                StartRandomPatrol();
                // Tăng tốc khi cả 2 trụ bị phá
                speed = brokenPillarsSpeed;
                randomPatrolSpeed = brokenPillarsSpeed;
            }
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
                ChangeState(BossRedState.Tran);

                hasStarted = true;

                Invoke(nameof(StartPatrol), 1.2f);
                canShoot = true;
            }
            else
            {
                StartPatrol();
                canShoot = true;
            }
        }

        void StartPatrol()
        {
            isActive = true;
            ChangeState(BossRedState.Walk);
        }

        // ---------------------------------------------------------
        // Player rời khỏi vùng → DỪNG bắn + dừng di chuyển
        // ---------------------------------------------------------
        public void StopBoss()
        {
            isActive = false;
            rb.linearVelocity = Vector2.zero;

            canShoot = false;
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

            //if (_state != BossRedState.Attack)
            //    ChangeState(BossRedState.Walk);

            if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
            {
                targetPoint = (targetPoint == pointA) ? pointB : pointA;
            }
        }


        // ---------------------------------------------------------
        // BẮN ĐẠN 4 HƯỚNG
        // ---------------------------------------------------------
        public void Shoot4Directions()
        {
            if (!canShoot) return;

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
        public void ChangeState(BossRedState newState)
        {
            //Reset state
            if (_state == BossRedState.Idle)
                anim.SetBool("idle", false);
            else if (_state == BossRedState.Attack)
                anim.SetBool("Attact", false);
            else if (_state == BossRedState.Tran)
                anim.SetBool("tran", false);
            else if (_state == BossRedState.Walk)
                anim.SetBool("Walk", false);
            else if (_state == BossRedState.Hit)
                anim.SetBool("Hit", false);

            //Gan' state moi'
            _state = newState;

            //Set bool animation theo state
            if (_state == BossRedState.Idle)
                anim.SetBool("idle", true);
            else if (_state == BossRedState.Attack)
                anim.SetBool("Attact", true);
            else if (_state == BossRedState.Tran)
                anim.SetBool("tran", true);
            else if (_state == BossRedState.Walk)
                anim.SetBool("Walk", true);
            else if (_state == BossRedState.Hit)
                anim.SetBool("Hit", true);
        }
        private Vector2 GetRandomPointInZone()// hàm chọn điểm random 
        {
            if (triggerZone == null) return transform.position; // fallback nếu chưa gán
            Bounds b = triggerZone.bounds;
            float x = Random.Range(b.min.x, b.max.x);
            float y = Random.Range(b.min.y, b.max.y);
            return new Vector2(x, y);
        }
        //Hàm khởi động random patrol
        public void StartRandomPatrol()
        {
            if (triggerZone == null) return;

            useRandomPatrol = true;
            currentTarget = GetRandomPointInZone();
            // Tăng tốc độ khi bắt đầu random patrol
            randomPatrolSpeed = brokenPillarsSpeed;
        }
        private void RandomPatrolUpdate()
        {
            if (!useRandomPatrol) return;

            Vector2 dir = (currentTarget - (Vector2)transform.position).normalized;
            rb.MovePosition(rb.position + dir * randomPatrolSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, currentTarget) < reachThreshold)
            {
                currentTarget = GetRandomPointInZone();
            }
        }
        // --------------- Reset trạng thái boss ----------------
        public void ResetBoss()
        {
            transform.position = pointA.position;
            targetPoint = pointA;
            useRandomPatrol = false;

            speed = 2f;
            randomPatrolSpeed = 2f;

            anim.enabled = true;
            rb.simulated = true;
            CancelInvoke(); // dừng bắn cũ

            pillarA.ResetPillar();
            pillarB.ResetPillar();

            isActive = false;
            hasStarted = false;

            ChangeState(BossRedState.Idle);
        }
    }

    public enum BossRedState
    {
        Idle,
        Tran,
        Attack,
        Walk,
        Hit
    }
}

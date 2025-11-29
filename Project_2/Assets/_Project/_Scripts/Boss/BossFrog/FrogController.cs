using _Project._Scripts.Core;
using _Project._Scripts.UI;
using System.Collections;
using TMPro;
using UnityEngine;

namespace _Project._Scripts.Enemies.Bosses
{
    public class FrogController : MonoBehaviour
    {
        public Animator anim;
        Rigidbody2D rb;
        Transform player;
        [SerializeField] BossState _state;

        [Header("Movement")]
        public float moveSpeed = 3f;

        [Header("Jump Attack Phase")]
        private const int minJumpCount = 3;
        private const int maxJumpCount = 6;
        int jumpCount;
        public float jumpDistance = 2f; // Khoảng cách mà boss sẽ nhảy mỗi lần
        public float jumpTime = 0.25f;// Thời gian để hoàn thành một cú nhảy

        [Header("Jump Attack")]
        [SerializeField] private Transform _attackPoint;
        [SerializeField] private float _attackRange = 2f;
        [SerializeField] private LayerMask _playerLayer;

        [Header("Skill Phase")]
        public float skillCooldown = 2f;

        [Header("Skill 1 Settings(spawn ếch con)")]
        public GameObject enemyPrefab;
        public Transform spawnPoint;
        public int spawnCount = 2;

        [Header("Skill 2 Settings (Bắn đạn ra 8 hướng)")]
        public GameObject bulletPrefab;
        public Transform shootPoint;
        public float bulletSpeed = 6f;
        public float bulletDamage = 5f;


        [Header("Skill 3 Settings (Freeze Zone)")]
        public GameObject freezeZonePrefab;
        public float freezeZoneDuration = 5f;
        public float freezeDuration = 5f;
        public float zoneRadius = 3f;

        private bool isActive = false;
        private bool isJumping = false;
        private int currentJump = 0;

        private Vector2 jumpStart;
        private Vector2 jumpEnd;
        private float jumpTimer = 0f;
        private Vector2 startPos;

        private int currentSkill = 0; // 1 = skill1, 2 = skill2, 3 = skill3

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            startPos = transform.position;

            if(player == null)
            {
                Transform Player = GameObject.FindWithTag("Player").transform;
                if(Player != null)
                {
                    player = Player;
                }
            }
        }

        private void Update()
        {
            if (_state == BossState.Jump)
            {
                // Nếu đang nhảy → di chuyển từ start → end
                if (isJumping)
                {
                    Flip();
                    GetComponent<FrogHealth>().SetCanTakeDamage(false);
                    jumpTimer += Time.deltaTime;
                    float t = Mathf.Clamp01(jumpTimer / jumpTime);
                    rb.position = Vector2.Lerp(jumpStart, jumpEnd, t);
                }
            }
            else if (_state == BossState.Idle || _state == BossState.Attack || _state == BossState.Hit)
            {
                rb.linearVelocity = Vector2.zero;
                GetComponent<FrogHealth>().SetCanTakeDamage(true);
            }
        }

        void Flip()
        {
            if(player.position.x < transform.position.x)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
        }

        public void OnPlayerEnter()
        {
            BGMController.Instance?.ChangeMusicMode(MusicMode.Battle);

            if (isActive) return;
            isActive = true;

            StopAllCoroutines();
            StartCoroutine(BossLoop());
        }

        public void OnPlayerExit()
        {
            BGMController.Instance?.ChangeMusicMode(MusicMode.Normal);

            isActive = false;
            ForceReturnToStart();
        }

        private IEnumerator BossLoop()
        {
            jumpCount = Random.Range(minJumpCount, maxJumpCount);

            while (isActive)
            {
                currentJump = 0;
                // Chạy tất cả nhảy
                StartCoroutine(JumpPhaseStep());

                while (currentJump < jumpCount)
                    yield return null;
                // Dừng nhảy, dùng skill
                yield return StartCoroutine(SkillPhase());
            }

            StartCoroutine(ReturnToStart());
        }

        private IEnumerator JumpPhaseStep()
        {
            if (!isActive || player == null || currentJump >= jumpCount)
                yield break;

            if (currentSkill != 0) // chặn nhảy khi skill 1
                yield break;

            ChangeState(BossState.Jump);
            isJumping = true;

            // tính điểm nhảy dựa trên vị trí player hiện tại
            jumpStart = transform.position;
            Vector2 dir = (player.position - transform.position).normalized;
            jumpEnd = jumpStart + dir * jumpDistance;
            jumpTimer = 0f;

            currentJump++;
            yield return null; // animation event sẽ gọi OnJumpLand
        }

        public void OnJumpLand()
        {
            isJumping = false;
            ChangeState(BossState.Idle);
            StartCoroutine(HoldIdleAndNextJump());
        }

        private IEnumerator HoldIdleAndNextJump()
        {
            yield return new WaitForSeconds(Random.Range(.5f, 2f));
            if (isActive && currentJump < jumpCount)
                StartCoroutine(JumpPhaseStep());
        }

        private IEnumerator SkillPhase()
        {
            // Chọn skill
            currentSkill = Random.Range(1, 4);
            Debug.Log("Boss dang dung skill " + currentSkill);

            // Play animation
            ChangeState(BossState.Attack);

            // CHỜ animation và hiệu ứng skill thực thi
            yield return new WaitForSeconds(skillCooldown);

            // Sau khi skill làm xong mới reset
            currentSkill = 0;
        }
        // Animation Event gọi ở frame tấn công
        public void SkillEffect()
        {
            switch (currentSkill)
            {
                case 1:
                    Skill1Spawn();
                    Debug.Log("Da dung skill 1");
                    break;
                case 2:
                    Skill2Burst();
                    Debug.Log("Da dung skill 2");
                    break;
                case 3:
                    Skill3Effect();
                    Debug.Log("su dung skill 3");
                    break;
            }
        }

        public void DealDamage()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRange, _playerLayer);

            foreach(var hit in hits)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if(damageable != null)
                {
                    damageable.TakeDamage(30);
                    DamagePopup.CreatePopup(hit.transform.position, 30, false);
                    IKnockbacked knockbacked = hit.GetComponent<IKnockbacked>();
                    if(knockbacked != null)
                    {
                        knockbacked.Knockback(transform, 1.2f, 1.2f);
                    }
                }
            }
        }

        // ==================== SKILL 1 (Animation Event gọi) ====================
        public void Skill1Spawn()
        {
            if (enemyPrefab == null || spawnPoint == null)
            {
                Debug.LogWarning("CHƯA GÁN enemyPrefab hoặc spawnPoint!");
                return;
            }

            Debug.Log("Skill1: Spawn enemy!");

            for (int i = 0; i < spawnCount; i++)
            {
                Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            }
        }
        // ==================== Skill 2 (Bắn đạn ra 8 hướng) ====================
        public void Skill2Burst()
        {
            if (bulletPrefab == null || shootPoint == null)
            {
                Debug.LogWarning("CHƯA GÁN bulletPrefab hoặc shootPoint!");
                return;
            }

            Vector2[] directions = new Vector2[]
            {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right,
        new Vector2(1, 1).normalized,
        new Vector2(1, -1).normalized,
        new Vector2(-1, 1).normalized,
        new Vector2(-1, -1).normalized
            };

            foreach (Vector2 dir in directions)
            {
                GameObject b = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
                Bullet bullet = b.GetComponent<Bullet>();
                bullet.direction = dir;
                bullet.speed = bulletSpeed;
                bullet.damage = bulletDamage;
            }
        }

        // ==================== Skill 3 (Player đứng yên) ====================
        public void Skill3Effect()
        {
            if (freezeZonePrefab == null)
            {
                Debug.LogWarning("CHƯA GÁN freezeZonePrefab!");
                return;
            }

            Vector2 dropOffset = Random.insideUnitCircle.normalized * Random.Range(3f, 5f);
            Vector2 freezePosition = (Vector2)transform.position + dropOffset;

            GameObject freezeZone = Instantiate(freezeZonePrefab, freezePosition, Quaternion.identity);

            Skill3Zone zoneScript = freezeZone.GetComponent<Skill3Zone>();
            zoneScript.duration = freezeZoneDuration;
            zoneScript.freezeDuration = freezeDuration;

            CircleCollider2D col = freezeZone.GetComponent<CircleCollider2D>();
            if (col != null)
                col.radius = zoneRadius;

            // tự destroy sau duration
            Destroy(freezeZone, zoneScript.duration);
        }


        private IEnumerator ReturnToStart()
        {
            Vector2 start = transform.position;
            Vector2 end = startPos;
            float t = 0f;

            ChangeState(BossState.Jump);
            while (t < 5f && start != end)
            {
                t += Time.deltaTime;
                transform.position = Vector2.Lerp(start, end, t / 5f);
                start = end;
                yield return null;
            }

            ChangeState(BossState.Idle);
        }
        public void ForceReturnToStart()
        {
            isActive = false;
            StopAllCoroutines();
            isJumping = false;

            StartCoroutine(ReturnToStart());
        }

        public void ChangeState(BossState newState)
        {
            //Reset state
            if (_state == BossState.Idle)
                anim.SetBool("isIdle", false);
            else if (_state == BossState.Jump)
                anim.SetBool("isJump", false);
            else if (_state == BossState.Attack)
                anim.SetBool("isAttack", false);
            else if (_state == BossState.Hit)
                anim.SetBool("isHit", false);
            else if (_state == BossState.ChargingAttack)
                anim.SetBool("isCharging", false);
            
            //Gan' state moi'
            _state = newState;

            //Set bool animation theo state
            if (_state == BossState.Idle)
                anim.SetBool("isIdle", true);
            else if (_state == BossState.Jump)
                anim.SetBool("isJump", true);
            else if (_state == BossState.Attack)
                anim.SetBool("isAttack", true);
            else if (_state == BossState.Hit)
                anim.SetBool("isHit", true);
            else if (_state == BossState.ChargingAttack)
                anim.SetBool("isCharging", true);
        }

        private void OnDrawGizmosSelected()
        {
            if (_attackPoint == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);
        }
    }

    public enum BossState { Idle, Jump, Attack, Hit, ChargingAttack }
}

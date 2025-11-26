using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace _Project._Scripts.Enemies.Bosses
{
    public class BossHealth : MonoBehaviour, IDamageable
    {

        [Header("UI Health Bar")]
        public Image healthFill;

        [Header("Stats")]
        public float maxHealth = 300f;
        private float currentHealth;

        [Header("Animator")]
        public Animator anim;

        [Header("Heal settings")]
        public float healPerSecond = 200f; // số máu hồi mỗi giây
        private bool canHeal => !pillarA.IsBroken || !pillarB.IsBroken;

        public event Action OnBossDead;

        private bool isDead = false;

        public BossPillar pillarA;
        public BossPillar pillarB;

        [Header("Respawn settings")]
        public float respawnTime = 2f; // thời gian hồi sinh

        BossController boss;

        private void Start()
        {
            boss = GetComponent<BossController>();
            currentHealth = maxHealth;

            if (healthFill != null)
            {
                healthFill.fillAmount = 1f;
                healthFill.transform.parent.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (canHeal && !isDead)
            {
                currentHealth += healPerSecond * Time.deltaTime;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

                if (healthFill != null)
                    healthFill.fillAmount = currentHealth / maxHealth;
            }
        }


        // ---------------------------
        // HIỆN THANH MÁU
        // ---------------------------
        public void ShowHealth()
        {
            if (healthFill != null)
                healthFill.transform.parent.gameObject.SetActive(true);
        }

        // ---------------------------
        // ẨN THANH MÁU
        // ---------------------------
        public void HideHealth()
        {
            if (healthFill != null)
                healthFill.transform.parent.gameObject.SetActive(false);
        }

        public void UpdateHealthBar()
        {
            if (healthFill == null) return;

            healthFill.fillAmount = currentHealth / maxHealth;
        }

        // ---------------------------
        // CHỊU SÁT THƯƠNG (Player gọi)
        // ---------------------------
        public void TakeDamage(float dmg)
        {
            if (isDead) return;

            // PlayerCombat gửi số âm → đổi lại thành dương
            float realDamage = -dmg;

            currentHealth -= realDamage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            // Gọi animation hit
            if (anim != null)
                boss.ChangeState(BossRedState.Hit);

            UpdateHealthBar();

            Debug.Log("Boss nhận damage: " + realDamage);

            if (currentHealth <= 0)
            {
                StartCoroutine(Die());
            }
        }

        // ---------------------------
        // BOSS CHẾT
        // ---------------------------
        private IEnumerator Die()
        {
            isDead = true;
            Debug.Log("BOSS DIE");

            HideHealth();

            yield return new WaitForSeconds(0.1f);

            OnBossDead?.Invoke();   // báo cho controller
            gameObject.SetActive(false); // tắt toàn bộ boss
        }

        public void Respawn()
        {
            isDead = false;
            currentHealth = maxHealth;
            UpdateHealthBar();

            // Reset boss
            boss.ResetBoss();

            gameObject.GetComponent<SpriteRenderer>().enabled = true;

            Debug.Log("BOSS HỒI SINH");
        }
        public void FullHeal()
        {
            isDead = false;
            currentHealth = maxHealth;
            UpdateHealthBar();
            GetComponent<SpriteRenderer>().enabled = true;
        }

    }
}

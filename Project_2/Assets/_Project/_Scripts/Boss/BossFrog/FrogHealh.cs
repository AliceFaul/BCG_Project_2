using UnityEngine;

public class FrogHealth : MonoBehaviour, IDamageable, IKnockbacked
{
    public float maxHealth = 200f;
    public float currentHealth;

    public GameObject healthUI;
    public UnityEngine.UI.Image healthBarFill;

    private Rigidbody2D rb;
    private bool isStunned = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
        HideHealth();
        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        float realDamage = -damage;
        currentHealth -= realDamage;

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateUI();

        // hiệu ứng bị đánh
        GetComponent<Animator>().SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Knockback(Transform attacker, float force, float stunTime)
    {
        if (rb == null) return;

        if (isStunned) return;

        isStunned = true;

        Vector2 dir = (transform.position - attacker.position).normalized;
        rb.linearVelocity = dir * force;

        // tắt stun sau stunTime
        Invoke(nameof(RemoveStun), stunTime);
    }

    private void RemoveStun()
    {
        isStunned = false;
        rb.linearVelocity = Vector2.zero;
    }

    private void UpdateUI()
    {
        if (healthBarFill)
            healthBarFill.fillAmount = currentHealth / maxHealth;
    }

    public void HideHealth()
    {
        if (healthUI) healthUI.SetActive(false);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
    public void ShowHealth()
    {
        if (healthUI) healthUI.SetActive(true);
    }

}

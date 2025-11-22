using UnityEngine;
using UnityEngine.UI;

public class BossPillar : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI Health Bar")]
    public Image healthFill; // Image dạng Filled

    public bool IsBroken => currentHealth <= 0;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float dmg)
    {
        if (IsBroken) return;

        // PlayerCombat gửi số âm đổi sang dương
        float realDamage = -dmg;
        currentHealth -= realDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(name + " nhận damage: " + realDamage + ", health còn lại: " + currentHealth);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Debug.Log(name + " bị phá!");
            Destroy(gameObject);
        }
    }

    private void UpdateHealthBar()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount = currentHealth / maxHealth;
        }
    }

    // Tùy chọn: hiện/ẩn thanh máu
    public void ShowHealth() => healthFill?.transform.parent.gameObject.SetActive(true);
    public void HideHealth() => healthFill?.transform.parent.gameObject.SetActive(false);
}

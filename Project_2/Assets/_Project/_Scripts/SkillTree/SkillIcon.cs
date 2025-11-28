using _Project._Scripts.Player;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    [Header("UI")]
    public Image skillImage;
    public Image cooldownOverlay;

    [Header("Skill Data")]
    private SkillData currentSkill;
    private bool isCoolingDown = false;

    private void Start()
    {
        if (skillImage == null) skillImage = GetComponent<Image>();
        ClearSkill();
    }

    // Gán kỹ năng vào ô
    public void SetSkill(SkillData data)
    {
        currentSkill = data;
        skillImage.sprite = data.normalSprite;
        skillImage.color = Color.white;
    }

    // Xóa kỹ năng khỏi ô
    public void ClearSkill()
    {
        currentSkill = null;
        skillImage.sprite = null;
        skillImage.color = Color.black;

        isCoolingDown = false;

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 0;
    }

    // Bấm skill
    public void OnClickSkill()
    {
        if (currentSkill == null || isCoolingDown)
            return;

        Debug.Log("Dùng kỹ năng: " + currentSkill.skillName);

        // TÌM PLAYER BẰNG TAG
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && currentSkill.skillEffectPrefab != null)
        {
            Vector3 pos = player.transform.position;

            // Tạo hiệu ứng skill
            GameObject effect = Instantiate(
                currentSkill.skillEffectPrefab,
                pos,
                Quaternion.identity
            );

            // muốn hiệu ứng đi theo player
            effect.transform.SetParent(player.transform);
            // Xử lý Animator 
            Animator anim = effect.GetComponent<Animator>();
            if (anim != null)
            {
                Debug.Log("Animaion is ready running");
                anim.Play(0);

                float animLength = 0f;
                if (anim.runtimeAnimatorController != null &&
                    anim.runtimeAnimatorController.animationClips.Length > 0)
                {
                    animLength = anim.runtimeAnimatorController.animationClips[0].length;
                }

                Destroy(effect, animLength); // Xóa sau khi chạy xong animation
            }
            else
            {
                Debug.LogWarning("Skill animator missing");
                Destroy(effect, currentSkill.effectDuration); // Nếu không có Animator
            }

            Debug.Log("Đã tạo hiệu ứng skill");
        }

        StartCoroutine(CooldownRoutine());
    }

    // Cooldown
    private IEnumerator CooldownRoutine()
    {
        isCoolingDown = true;

        skillImage.sprite = currentSkill.usedSprite;

        float time = currentSkill.cooldown;
        float start = Time.realtimeSinceStartup;
        float end = start + time;

        while (Time.realtimeSinceStartup < end)
        {
            float remaining = end - Time.realtimeSinceStartup;

            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = remaining / time;

            yield return null;
        }

        // Hoàn tất cooldown
        skillImage.sprite = currentSkill.normalSprite;

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 0;

        isCoolingDown = false;

        Debug.Log("Cooldown xong");
    }

    public SkillData GetSkillData()
    {
        return currentSkill;
    }
}

using _Project._Scripts.Player;
using System;
using System.Collections;
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

    [Header("Hotkey")]
    public KeyCode hotkey;   // <- PHÍM DÙNG SKILL

    private void Start()
    {
        if (skillImage == null) skillImage = GetComponent<Image>();
        ClearSkill();
    }
    private void Update()
    {
        // --- DÙNG PHÍM ---
        if (currentSkill != null && !isCoolingDown)
        {
            if (Input.GetKeyDown(hotkey))
            {
                OnClickSkill();
            }
        }
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
        skillImage.color = new Color(1, 1, 1, 0);

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

        // Lấy player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Không tìm thấy Player!");
            return;
        }

        Vector3 spawnPos = Vector3.zero;

        //========== XÁC ĐỊNH VỊ TRÍ SPAWN ==========
        if (currentSkill.spawnType == SkillSpawnType.AtPlayer)
        {
            spawnPos = player.transform.position;
        }
        else if (currentSkill.spawnType == SkillSpawnType.AtMouse)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f; // khoảng cách từ camera đến world
            spawnPos = Camera.main.ScreenToWorldPoint(mousePos);
        }


        //========== GỌI LOGIC SKILL CỦA PLAYER ==========
        SkillExecutor executor = player.GetComponent<SkillExecutor>();
        if (executor != null)
        {
            executor.ExecuteSkill(currentSkill, spawnPos);
        }
        else
        {
            Debug.LogWarning("Player không có SkillExecutor!");
        }


        //========== TẠO HIỆU ỨNG VFX ========== 
        if (currentSkill.skillEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                currentSkill.skillEffectPrefab,
                spawnPos,
                Quaternion.identity
            );

            // Nếu skill sinh tại người → effect follow player
            if (currentSkill.spawnType == SkillSpawnType.AtPlayer)
            {
                effect.transform.SetParent(player.transform);
            }

            Animator anim = effect.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Play(0);

                // Lấy độ dài animation
                float animLength = 0f;
                if (anim.runtimeAnimatorController != null &&
                    anim.runtimeAnimatorController.animationClips.Length > 0)
                {
                    animLength = anim.runtimeAnimatorController.animationClips[0].length;
                }

                Destroy(effect, animLength);
            }
            else
            {
                // Nếu không có animator → dùng thời gian trong SkillData
                Destroy(effect, currentSkill.effectDuration);
            }
        }


        //========== BẮT ĐẦU COOLDOWN ==========
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

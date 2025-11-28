using _Project._Scripts.Player;
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

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector3 pos = Vector3.zero;

        //---------- Xác định vị trí spawn ----------
        if (currentSkill.spawnType == SkillSpawnType.AtPlayer)
        {
            pos = player.transform.position;
        }
        else if (currentSkill.spawnType == SkillSpawnType.AtMouse)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f;
            pos = Camera.main.ScreenToWorldPoint(mousePos);
        }

        //---------- Tạo hiệu ứng ----------
        if (currentSkill.skillEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                currentSkill.skillEffectPrefab,
                pos,
                Quaternion.identity
            );

            // Follow player nếu spawn tại người chơi
            if (currentSkill.spawnType == SkillSpawnType.AtPlayer)
                effect.transform.SetParent(player.transform);

            Animator anim = effect.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Play(0);

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
                Destroy(effect, currentSkill.effectDuration);
            }
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

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

    private void Start()
    {
        if (skillImage == null) skillImage = GetComponent<Image>();
        ClearSkill();
    }

    public void SetSkill(SkillData data)
    {
        currentSkill = data;
        skillImage.sprite = data.normalSprite;
        skillImage.color = Color.white;
    }

    public void ClearSkill()
    {
        currentSkill = null;
        skillImage.sprite = null;
        skillImage.color = new Color(1, 1, 1, 0);
        isCoolingDown = false;
        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 0;
    }

    public void OnClickSkill()
    {
        if (currentSkill == null || isCoolingDown) return;

        Debug.Log("Dùng ky nãng: " + currentSkill.skillName);
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        isCoolingDown = true;
        skillImage.sprite = currentSkill.usedSprite;

        float time = currentSkill.cooldown;
        float startTime = Time.realtimeSinceStartup; 
        float endTime = startTime + time;

        while (Time.realtimeSinceStartup < endTime)
        {
            float remaining = endTime - Time.realtimeSinceStartup;
            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = remaining / time;

            yield return null;
        }

       
        skillImage.sprite = currentSkill.normalSprite;
        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 0;

        isCoolingDown = false;
        Debug.Log("da chuyen lai anh binh thuong");
    }
    public SkillData GetSkillData()
    {
        return currentSkill;
    }

}

using UnityEngine;
using UnityEngine.UI;

public class SkillActionPanel : MonoBehaviour
{
    public Button useButton;
    public Button dropButton;

    private SkillData currentSkill;

    private void Start()
    {
        gameObject.SetActive(false);

        useButton.onClick.AddListener(OnUseSkill);
        dropButton.onClick.AddListener(OnDropSkill);
    }

    public void Show(SkillData skill)
    {
        currentSkill = skill;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        currentSkill = null;
        gameObject.SetActive(false);
    }

    private void OnUseSkill()
    {
        if (currentSkill == null) return;
        UIManager.Instance.AssignSkillToHud(currentSkill);
        Hide();
    }

    private void OnDropSkill()
    {
        if (currentSkill == null) return;
        UIManager.Instance.RemoveSkillFromHud(currentSkill);
        Hide();
    }
}

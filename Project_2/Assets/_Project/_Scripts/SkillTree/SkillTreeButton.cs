using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SkillTreeButton : MonoBehaviour
{
    public SkillDescriptionUI descriptionUI;
    public SkillData skillData;
    private Button button;

    private void Awake()
    { 
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickSkill);
    }

    private void OnClickSkill()
    {
        // hiển thị mô tả kỹ năng 
        if (UIManager.Instance != null)
            UIManager.Instance.ShowSkillDescription(skillData);

         //Gán kỹ năng ra HUD
        if (UIManager.Instance != null)
            UIManager.Instance.AssignSkillToHud(skillData);

        Debug.Log($"Đã chọn kỹ năng: {skillData.skillName}");
    }
}

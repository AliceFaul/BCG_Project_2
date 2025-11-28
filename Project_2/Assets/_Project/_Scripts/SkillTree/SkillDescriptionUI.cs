using UnityEngine;
using TMPro;

public class SkillDescriptionUI : MonoBehaviour
{
    [Header("UI hien thi mo ta")]
    public TextMeshProUGUI descriptionText;
    public void ShowDescription(SkillData data)
    {
        if (data == null)
        {
            descriptionText.text = "";
            return;
        }
        descriptionText.text = data.skillDescription;
    }
}

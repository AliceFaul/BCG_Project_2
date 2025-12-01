using _Project._Scripts.Player;
using _Project._Scripts.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public SkillIcon[] hudSlots;
    public SkillDescriptionUI descriptionUI;
    public SkillActionPanel actionPanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowSkillDescription(SkillData skill)
    {
        if (skill == null) return;

        // Lấy điều kiện từ player
        int playerLevel = HUDController.Instance != null ? HUDController.Instance.CurrentLevel : 0;
        int playerCoins = PlayerWallet.Instance != null ? PlayerWallet.Instance.Coins : 0;

        bool enoughLevel = playerLevel >= skill.requiredLevel;
        bool enoughCoin = playerCoins >= skill.requiredCoin;

        // Thay thế ShowDescription bằng ShowSkill
        if (descriptionUI != null)
            descriptionUI.ShowSkill(skill, enoughLevel, enoughCoin);

        if (actionPanel != null)
            actionPanel.Show(skill);
    }

    private void Start()
    {
        if (hudSlots.Length >= 6)
        {
            hudSlots[0].hotkey = KeyCode.Z;
            hudSlots[1].hotkey = KeyCode.X;
            hudSlots[2].hotkey = KeyCode.C;
            hudSlots[3].hotkey = KeyCode.V;
            hudSlots[4].hotkey = KeyCode.G;
            hudSlots[5].hotkey = KeyCode.T;
        }
    }
    public void AssignSkillToHud(SkillData skill)
    {
        foreach (SkillIcon slot in hudSlots)
        {
            if (slot != null && slot.GetSkillData() == null)
            {
                slot.SetSkill(skill);
                Debug.Log($"Đã gán kỹ năng {skill.skillName} ra HUD!");
                return;
            }
        }

        Debug.Log("Không còn ô trống ngoài HUD!");
    }
    public void RemoveSkillFromHud(SkillData skill)
    {
        foreach (SkillIcon slot in hudSlots)    
        {
            if (slot != null && slot.GetSkillData() == skill)
            {
                slot.ClearSkill();
                Debug.Log($"Đã gỡ kỹ năng {skill.skillName} khỏi HUD!");
                return;
            }
        }
        Debug.Log("Kỹ năng này không có trong HUD!");
    }
}

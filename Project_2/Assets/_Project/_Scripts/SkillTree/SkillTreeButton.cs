using UnityEngine;
using UnityEngine.UI;
using _Project._Scripts.Player;
using _Project._Scripts.UI;

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
        int playerLevel = HUDController.Instance != null ? HUDController.Instance.CurrentLevel : 0;
        int playerCoins = PlayerWallet.Instance != null ? PlayerWallet.Instance.Coins : 0;

        bool enoughLevel = playerLevel >= skillData.requiredLevel;
        bool enoughCoin = playerCoins >= skillData.requiredCoin;

        // Nếu khóa → không cho mở panel hành động
        if (!enoughLevel || !enoughCoin)
        {
            Debug.Log($"Không đủ điều kiện: Level cần {skillData.requiredLevel}, Coin cần {skillData.requiredCoin}");
            UIManager.Instance.actionPanel.Hide();
            return;
        }

        // Đủ điều kiện -> hiển thị panel
        UIManager.Instance.actionPanel.Show(skillData);
        Debug.Log($"Đã chọn kỹ năng: {skillData.skillName}");
    }

}

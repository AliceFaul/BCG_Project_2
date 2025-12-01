using UnityEngine;
using UnityEngine.UI;
using _Project._Scripts.UI;
using _Project._Scripts.Player;

[RequireComponent(typeof(Button))]
public class SkillTreeButton : MonoBehaviour
{
    public SkillDescriptionUI descriptionUI;   // Panel hiển thị mô tả
    public SkillData skillData;                // Data của skill này

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickSkill);
    }

    private void OnClickSkill()
    {
        if (skillData == null)
        {
            Debug.LogWarning("SkillTreeButton: SkillData chưa được gán!");
            return;
        }

        if (descriptionUI == null)
        {
            Debug.LogWarning("SkillTreeButton: descriptionUI chưa gán!");
            return;
        }

        // Lấy level và coin từ hệ thống
        int playerLevel = HUDController.Instance != null ? HUDController.Instance.CurrentLevel : 0;
        int playerCoins = PlayerWallet.Instance != null ? PlayerWallet.Instance.Coins : 0;

        bool enoughLevel = playerLevel >= skillData.requiredLevel;
        bool enoughCoin = playerCoins >= skillData.requiredCoin;

        // Hiển thị mô tả + xử lý nút unlock/use/drop
        descriptionUI.ShowSkill(skillData, enoughLevel, enoughCoin);

        // Hiển thị panel UI, nếu bạn có panel
        if (UIManager.Instance != null && UIManager.Instance.actionPanel != null)
        {
            UIManager.Instance.actionPanel.Show(skillData);
        }

        Debug.Log($"Đã chọn kỹ năng: {skillData.skillName} | Level: {playerLevel}, Coin: {playerCoins}");
    }
}

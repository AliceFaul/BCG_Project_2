using UnityEngine;
using TMPro;
using UnityEngine.UI;
using _Project._Scripts.Player;

public class SkillDescriptionUI : MonoBehaviour
{
    [Header("UI hiển thị")]
    public TextMeshProUGUI descriptionText;

    [Header("Icon hiển thị")]
    public Image skillIconImage;


    [Header("Các nút điều khiển")]
    public Button unlockButton;
    public Button useButton;
    public Button dropButton;

    private SkillData currentSkill;
    private bool canUnlock = false;

    // Hàm được gọi từ SkillTreeButton
    public void ShowSkill(SkillData data, bool enoughLevel, bool enoughCoin)
    {
        currentSkill = data;

        if (data == null)
        {
            descriptionText.text = "";
            return;
        }

        // Hiển thị mô tả
        descriptionText.text = data.skillDescription;
        // Cập nhật icon
        UpdateSkillSprite(currentSkill);


        // Nếu đã mở -> chỉ hiện Use/Drop
        if (data.state == State.Unlocked)
        {
            unlockButton.gameObject.SetActive(false);
            useButton.gameObject.SetActive(true);
            dropButton.gameObject.SetActive(true);
            return;
        }

        // Nếu chưa mở khóa
        canUnlock = enoughLevel && enoughCoin;

        unlockButton.gameObject.SetActive(true);
        useButton.gameObject.SetActive(false);
        dropButton.gameObject.SetActive(false);

        // Nút Unlock sáng hay mờ
        unlockButton.interactable = canUnlock;

        // Gán sự kiện nút Unlock
        unlockButton.onClick.RemoveAllListeners();
        unlockButton.onClick.AddListener(OnUnlockSkill);
    }

    private void OnUnlockSkill()
    {
        if (!canUnlock)
        {
            Debug.Log("Không đủ điều kiện mở kỹ năng!");
            return;
        }

        // Trừ tiền
        PlayerWallet.Instance.SpendCoin(currentSkill.requiredCoin);

        // Đổi state
        currentSkill.state = State.Unlocked;

        UpdateSkillSprite(currentSkill);

        Debug.Log($"Đã mở khóa kỹ năng: {currentSkill.skillName}");

        // Cập nhật UI
        unlockButton.gameObject.SetActive(false);
        useButton.gameObject.SetActive(true);
        dropButton.gameObject.SetActive(true);
    }

    public void UpdateSkillSprite(SkillData data)
    {
        if (data == null || skillIconImage == null) return;
        // Luôn dùng usedSprite khi Locked, normalSprite khi Unlocked
        skillIconImage.sprite = (data.state == State.Unlocked) ? data.normalSprite : data.usedSprite;
        // Cập nhật icon theo state
        if (data.state == State.Unlocked)
        {
            skillIconImage.sprite = data.normalSprite; // hiển thị màu
        }
        else if(data.state == State.Locked)
        {
            skillIconImage.sprite = data.usedSprite; // hiển thị trắng đen
        }
    }

}
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Game/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("Thông tin ")]
    public string skillName;

    [TextArea(2, 5)]
    public string skillDescription;

    [Header("Hình ảnh kỹ năng")]
    public Sprite normalSprite;   // anh chưa dùng 
    public Sprite usedSprite;     // anh khi dùng(trang đen)


    [Header("Thông số kỹ năng")]
    public float cooldown = 5f;   // thoi gian hoi chiêu 
    public int requiredLevel = 1; // level mo khóa
}

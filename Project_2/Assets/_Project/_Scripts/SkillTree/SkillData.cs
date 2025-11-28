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

    [Header("Điều kiện mở khóa")]
    public int requiredLevel = 1;
    public int requiredCoin = 0;

    [Header("Hiệu ứng kỹ năng")]
    public GameObject skillEffectPrefab;
    public float effectDuration = 10f; //thoi gian ton tai hieu ung 

    [Header("Kiểu xuất hiện")]
    public SkillSpawnType spawnType = SkillSpawnType.AtPlayer;
}
public enum SkillSpawnType
{
    AtPlayer,
    AtMouse
}

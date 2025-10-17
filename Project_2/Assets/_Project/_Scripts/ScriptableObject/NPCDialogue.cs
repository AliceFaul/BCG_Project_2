using UnityEngine;

/// <summary>
/// Sử dụng ScriptableObject để lưu các thông tin của NPC thành một dữ liệu tĩnh 
/// và sử dụng trong script NPC
/// </summary>
[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPCDialogue")]
public class NPCDialogue : ScriptableObject
{
    //Thiết lập các thông tin và lời nói của NPC
    public string _npcName; //Tên của NPC
    public Sprite _npcPortrait; //Ảnh chân dung của NPC đang nói
    public string[] _dialogueLines; //Là những lời nói của NPC 
    public float _typingSpeed = 0.05f; //Tốc độ hiện chữ (sử dụng cho auto nói chuyện)
    public AudioClip _voiceSound; //Âm thanh khi NPC nói
    public float _voicePitch = 1f; //Tần số của AudioSource

    //Các biến bool kiểm soát hành vi khi đang diễn ra Dialogue Event
    public bool[] _autoProgressLines; //Tick true sẽ tự động chạy hội thoại index nào sẽ tương đương với index trong dialogueLines
    public float _autoProgressDelay = 2f; //Là thời gian sẽ tự động chuyển sang đoạn hội thoại tiếp theo
}

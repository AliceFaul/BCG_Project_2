using UnityEngine;

/// <summary>
/// Sử dụng ScriptableObject để lưu các thông tin của NPC thành một dữ liệu tĩnh 
/// và sử dụng trong script NPC
/// </summary>
[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPCDialogue")]
public class NPCDialogue : ScriptableObject
{
    [Header("Thiết lập thông tin của Player")]
    public string _playerName = "Froggo";
    public bool _firstLineIsPlayer;

    //Thiết lập các thông tin và lời nói của NPC
    public string _npcName; //Tên của NPC
    public Sprite _npcPortrait; //Ảnh chân dung của NPC đang nói
    public string[] _dialogueLines; //Là những lời nói của NPC 
    public float _typingSpeed = 0.05f; //Tốc độ hiện chữ (sử dụng cho auto nói chuyện)
    public AudioClip _voiceSound; //Âm thanh khi NPC nói
    public float _voicePitch = 1f; //Tần số của AudioSource

    //Các biến bool kiểm soát hành vi khi đang diễn ra Dialogue Event
    public bool[] _autoProgressLines; //Tick true sẽ tự động chạy hội thoại index nào sẽ tương đương với index trong dialogueLines
    public bool[] _endProgressLines; //Tick true sẽ tự động ngắt cuộc hội thoại tại index đó 
    public float _autoProgressDelay = 2f; //Là thời gian sẽ tự động chuyển sang đoạn hội thoại tiếp theo

    public DialogueChoice[] _choices; //Đây là những lựa chọn của người chơi

    public int _questInProgressIndex; //Index của đoạn hội thoại quest đã nhận nhưng chưa hoàn thành (In Progress)
    public int _questCompletedIndex; //Index của đoạn hội thoại quest đã hoàn thành
    public Quest _quest; //Là ScriptableObject dữ liệu quest của NPC, có nghĩa là quest mà NPC sẽ giao cho mình
}

[System.Serializable]
public class DialogueChoice
{
    [Tooltip("Index trong DialogueLines sẽ xuất hiện choice")]
    public int _dialogueIndex; //Index chỉ ra đến đoạn hội thoại nào sẽ xuất hiện Choice
    [Tooltip("Là câu trả lời của người chơi")]
    public string[] _choices; //Mảng string này sẽ lưu câu trả lời của người chơi
    [Tooltip("Lời thoại của player tương ứng với lựa chọn")]
    public string[] _playerReplyLines;
    [Tooltip("Index của đoạn sẽ từ lựa chọn tương đương với thứ tự trong mảng choices ở trên")]
    public int[] _nextDialogueIndexes; //Index sẽ từ lựa chọn của người chơi mà sẽ chỉ tới đoạn hội thoại của câu trả lời đó
    public bool[] _giveQuests; //Tick true có nghĩa là lựa chọn đó sẽ cho quest (Thứ tự tương đương với _choices) 
}

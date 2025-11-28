using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace _Project._Scripts.Core
{
    /// <summary>
    /// Script kiểm soát UI Dialogue và sử dụng ScriptableObject NPCDialogue để thực hiện hội thoại với người chơi
    /// </summary>
    public class DialogueController : MonoBehaviour
    {
        public static DialogueController Instance { get; private set; } //Singleton của DialogueController

        [Header("Những thành phần trong Dialogue UI")]
        [SerializeField] public GameObject _dialoguePanel; //Là UI Dialogue
        [SerializeField] public TMP_Text _dialogueText, _nameText; //Hội thoại và tên NPC trong UI và được chỉnh sửa nội dung bằng NPCDialogue
        [SerializeField] public Image _npcPortrait, _playerPortrait; //Là ảnh chân dung của NPC và cũng được chỉnh sửa theo NPCDialogue

        [SerializeField] private Transform _choiceContainer; //Là panel chứa choice button
        [SerializeField] private GameObject _pfChoiceButton; //Prefab choice button

        [SerializeField] private GameObject _hotbar;

        Color _bright = Color.white;
        Color _dim = new Color(1, 1, 1, .4f);

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            //Thiết lập Singleton của DialogueController
            //Singleton sẽ gọi qua các script khác dùng Dialogue
            if(Instance == null) Instance = this;
            Destroy(gameObject);
        }

        public void HideHotbar(bool enable) => _hotbar.SetActive(!enable);

        #region Setting Info UI Dialogue

        /// <summary>
        /// Hàm xuất hiện ra Dialogue UI và thiết lập các thông tin trong UI
        /// </summary>
        public void ShowDialogueUI(bool show)
        {
            _dialoguePanel.SetActive(show);
        }

        public void SetUpPlayerInfo(string name)
        {
            _nameText.text = name;
        }

        //Hàm phụ trách việc thiết lập các thông tin như tên và ảnh chân dung
        public void SetupInfo(string name, Sprite portrait)
        {
            _nameText.text = name;
            _npcPortrait.sprite = portrait;
        }

        //Hàm phụ trách việc thiết lập lời thoại
        public void SetupDialogueText(string text)
        {
            _dialogueText.text = text;
        }

        #region Speaker Mode Setting

        /// <summary>
        /// NPC nói chuyện - Tắt dialogueBox của Player, Portrait Player tối lại
        /// </summary>
        public void ShowNPCSpeaking()
        {
            _npcPortrait.color = _bright;
            _playerPortrait.color = _dim;
        }

        /// <summary>
        /// Player nói chuyện - Tắt dialogueBox của NPC, Portrait NPC tối lại
        /// </summary>
        public void ShowPlayerSpeaking()
        {
            _npcPortrait.color = _dim;
            _playerPortrait.color = _bright;
        }

        #endregion

        #endregion

        #region Setting Dialogue Branch Choice

        /// <summary>
        /// Hàm giúp clear sạch trong choiceContainer (giống kiểu refresh lại)
        /// </summary>
        public void ClearChoice()
        {
            foreach(Transform child in _choiceContainer)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Hàm giúp tạo choiceButton bằng prefab trong choiceContainer
        /// </summary>
        /// <param name="choiceText"></param>
        /// <param name="onclick"></param>
        /// <returns></returns>
        public GameObject CreateChoice(string choiceText, UnityAction onclick)
        {
            GameObject choiceButton = Instantiate(_pfChoiceButton, _choiceContainer);
            choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText;
            choiceButton.GetComponent<Button>().onClick.AddListener(onclick);
            return choiceButton;
        }

        #endregion
    }
}

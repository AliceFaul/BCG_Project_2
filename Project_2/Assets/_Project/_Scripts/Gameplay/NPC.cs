using System.Collections;
using _Project._Scripts.Core;
using _Project._Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.Gameplay
{
    public class NPC : MonoBehaviour, IInteractable
    {
        [Header("Các Property của một NPC")]
        [Tooltip("Là ScriptableObject NPCDialogue được cài đặt trong Unity")]
        [SerializeField] private NPCDialogue _dialogueData;
        [SerializeField] private GameObject _dialoguePanel; //Là UI Dialogue 
        [SerializeField] private TMP_Text _dialogueText, _nameText; //Hội thoại và tên NPC trong UI và được chỉnh sửa nội dung bằng NPCDialogue
        [SerializeField] private Image _npcPortrait; //Là ảnh chân dung của NPC và cũng được chỉnh sửa theo NPCDialogue

        //Các biến nội bộ của NPC
        private int _dialogueIndex; //Index của đoạn hội thoại, biến này sẽ cho biết đoạn hội thoại hiện tại đang hiện ra
        private bool _isTyping, _isDialogueActive;

        #region NPC Interact Life Cycle

        public bool CanInteract()
        {
            return !_isDialogueActive;
        }

        //Hàm tương tác và bắt đầu Dialogue
        public void Interact()
        {
            if (_dialogueData == null || (PauseController.IsGamePaused && !_isDialogueActive))
                return;

            if(_isDialogueActive)
            {
                NextLine();
            }
            else
            {
                StartDialogue();
            }
        }

        #endregion

        #region NPC Dialogue System

        //Hàm sẽ bật UI Dialogue và set name và portrait giống trong dialogueData
        void StartDialogue()
        {
            _isDialogueActive = true; //Cho biết dialogue đang được bật
            _dialogueIndex = 0; //Index set bằng 0 để bắt đầu với line đầu tiên

            //Set name và portrait theo dialogueData
            _nameText.SetText(_dialogueData._npcName);
            _npcPortrait.sprite = _dialogueData._npcPortrait;

            //Bật UI dialogue và bật pause
            _dialoguePanel.SetActive(true);
            PauseController.SetPaused(true);
            HUDController.Instance.HidePlayerHUD(true);

            //Bắt đầu hiện ra nội dung hội thoại
            StartCoroutine(TypeLine());
        }

        //Hàm này sẽ xem xem có còn lines nào trong dialogueData không và hiện ra lines tiếp theo
        void NextLine()
        {
            if(_isTyping)
            {
                StopAllCoroutines();
                _dialogueText.SetText(_dialogueData._dialogueLines[_dialogueIndex]);
                _isTyping = false;
            }
            else if(++_dialogueIndex < _dialogueData._dialogueLines.Length)
            {
                StartCoroutine(TypeLine());
            }
            else
            {
                EndDialogue();
            }
        }

        //Coroutine để chạy từng chữ
        IEnumerator TypeLine()
        {
            //isTyping = true cho biết nội dung đang được nhập và clean text trong UI 
            _isTyping = true;
            _dialogueText.SetText("");

            //Dựa vào index để hiện ra text trong dialogueData
            foreach(char letter in _dialogueData._dialogueLines[_dialogueIndex])
            {
                _dialogueText.text += letter;
                yield return new WaitForSeconds(_dialogueData._typingSpeed); //Nhập với tốc độ của dialogueData
            }

            _isTyping = false;

            //Kiểm tra xem còn line tiếp theo không và có bật auto không để tự động hiện ra line tiếp theo
            if(_dialogueData._autoProgressLines.Length > _dialogueIndex && 
                _dialogueData._autoProgressLines[_dialogueIndex])
            {
                yield return new WaitForSeconds(_dialogueData._autoProgressDelay);
                NextLine();
            }
        }

        //Hàm kết thúc dialogue event
        void EndDialogue()
        {
            StopAllCoroutines();
            _isDialogueActive = false;
            _dialogueText.SetText("");
            _dialoguePanel.SetActive(false);
            PauseController.SetPaused(false);
            HUDController.Instance.HidePlayerHUD(false);
        }

        #endregion
    }
}

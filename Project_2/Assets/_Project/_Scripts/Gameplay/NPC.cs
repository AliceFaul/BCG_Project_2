using System.Collections;
using _Project._Scripts.Core;
using _Project._Scripts.UI;
using UnityEngine;

namespace _Project._Scripts.Gameplay
{
    public class NPC : MonoBehaviour, IInteractable
    {
        [Header("Các Property của một NPC")]
        [Tooltip("Là ScriptableObject NPCDialogue được cài đặt trong Unity")]
        [SerializeField] private NPCDialogue _dialogueData;
        DialogueController _dialogueUI;

        //Các biến nội bộ của NPC
        private int _dialogueIndex; //Index của đoạn hội thoại, biến này sẽ cho biết đoạn hội thoại hiện tại đang hiện ra
        private bool _isTyping, _isDialogueActive;

        //Biến trạng thái enum của quest ở NPC này
        private QuestState _questState = QuestState.NotStarted;

        #region NPC Interact Life Cycle

        private void Start()
        {
            _dialogueUI = DialogueController.Instance;
        }

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
            SyncQuestState();

            if(_questState == QuestState.NotStarted)
            {
                _dialogueIndex = 0; //Index set bằng 0 để bắt đầu với line đầu tiên
            }
            else if(_questState == QuestState.InProgress)
            {
                _dialogueIndex = _dialogueData._questInProgressIndex;
            }
            else if(_questState == QuestState.Completed)
            {
                _dialogueIndex = _dialogueData._questCompletedIndex;
            }

            _isDialogueActive = true; //Cho biết dialogue đang được bật

            //Set name và portrait theo dialogueData
            _dialogueUI.SetupInfo(_dialogueData._npcName, _dialogueData._npcPortrait);

            //Bật UI dialogue và bật pause
            _dialogueUI.ShowDialogueUI(true);
            PauseController.SetPaused(true);
            HUDController.Instance.HidePlayerHUD(true);

            //Bắt đầu hiện ra nội dung hội thoại
            DisplayCurrentLine();
        }

        void SyncQuestState()
        {
            if (_dialogueData._quest == null) return;

            string questID = _dialogueData._quest._questID;
            if(QuestController.Instance.IsQuestActive(questID))
            {
                _questState = QuestState.InProgress;
            }
            else
            {
                _questState = QuestState.NotStarted;
            }
        }

        //Hàm này sẽ xem xem có còn lines nào trong dialogueData không và hiện ra lines tiếp theo
        void NextLine()
        {
            if(_isTyping)
            {
                StopAllCoroutines();
                _dialogueUI.SetupDialogueText(_dialogueData._dialogueLines[_dialogueIndex]);
                _isTyping = false;
            }

            //Ở đoạn hội thoại tiếp theo nếu có lựa chọn thì sẽ clear lựa chọn cũ
            //Sau đó sẽ tạo lại lựa chọn mới
            _dialogueUI.ClearChoice();

            //Kiểm tra trong ScriptableObject _endProgressLines có tick true không
            //Nếu có thì sẽ kết thúc hội thoại ở đoạn đó nếu không thì sẽ tiếp tục
            if(_dialogueData._endProgressLines.Length > _dialogueIndex &&
                _dialogueData._endProgressLines[_dialogueIndex])
            {
                EndDialogue();
                return;
            }

            //Lúc này ta sẽ kiểm tra nếu index trong DialogueChoice bằng với _dialogueIndex trong Script này
            //Thì sẽ tạo ra các button choice trong choiceContainer
            //và nếu chọn button nào sẽ thay đổi _dialogueIndex dẫn đến đoạn hội thoại của button đó
            //Còn nếu đoạn hội thoại này không có lựa chọn (dialogueChoice._dialogueIndex != _dialogueIndex)
            //Thì sẽ xuất ra đoạn hội thoại tiếp theo như bình thường
            foreach(DialogueChoice dialogueChoice in _dialogueData._choices)
            {
                if(dialogueChoice._dialogueIndex == _dialogueIndex)
                {
                    DisplayChoices(dialogueChoice);
                    return;
                }
            }

            //Xuất ra đoạn hội thoại tiếp theo bình thường theo _dialogueIndex
            if(++_dialogueIndex < _dialogueData._dialogueLines.Length)
            {
                DisplayCurrentLine();
            }
            else
            {
                EndDialogue();
            }
        }

        #region Choice Settings

        //Hàm này sẽ tạo ra các button lựa chọn cùng với thay đổi text trong lựa chọn và event onClick của button
        //Event onClick sẽ giúp thay đổi _dialogueIndex và xuất ra đoạn hội thoại tiếp theo
        void DisplayChoices(DialogueChoice choice)
        {
            for(int i = 0; i < choice._choices.Length; i++)
            {
                int nextIndex = choice._nextDialogueIndexes[i];
                bool giveQuest = choice._giveQuests[i];
                _dialogueUI.CreateChoice(choice._choices[i], () => ChooseOption(nextIndex, giveQuest));
            }
        }

        //Hàm này thay đổi _dialogueIndex sau đó xóa lựa chọn và hiện ra đoạn hội thoại với _dialogueIndex mới
        void ChooseOption(int nextIndex, bool giveQuest)
        {
            if(giveQuest)
            {
                QuestController.Instance.AcceptQuest(_dialogueData._quest);
                _questState = QuestState.InProgress;
            }

            _dialogueIndex = nextIndex;
            _dialogueUI.ClearChoice();
            DisplayCurrentLine();
        }

        #endregion

        void DisplayCurrentLine()
        {
            StopAllCoroutines();
            StartCoroutine(TypeLine());
        }

        //Coroutine để chạy từng chữ
        IEnumerator TypeLine()
        {
            //isTyping = true cho biết nội dung đang được nhập và clean text trong UI 
            _isTyping = true;
            _dialogueUI.SetupDialogueText("");

            //Dựa vào index để hiện ra text trong dialogueData
            foreach(char letter in _dialogueData._dialogueLines[_dialogueIndex])
            {
                _dialogueUI.SetupDialogueText(_dialogueUI._dialogueText.text += letter);
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
            _dialogueUI.SetupDialogueText("");
            _dialogueUI.ShowDialogueUI(false);
            PauseController.SetPaused(false);
            HUDController.Instance.HidePlayerHUD(false);
        }

        #endregion
    }

    public enum QuestState { NotStarted, InProgress, Completed }
}

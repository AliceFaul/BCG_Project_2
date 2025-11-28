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

        private bool _isPlayerSpeaking;

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

            //Bật UI dialogue và bật pause
            _dialogueUI.ShowDialogueUI(true);
            _dialogueUI.HideHotbar(true);
            PauseController.SetPaused(true);
            HUDController.Instance.HidePlayerHUD(true);

            _isPlayerSpeaking = _dialogueData._firstLineIsPlayer;

            if(_isPlayerSpeaking)
                _dialogueUI.SetUpPlayerInfo(_dialogueData._playerName);
            else
                _dialogueUI.SetupInfo(_dialogueData._npcName, _dialogueData._npcPortrait);

            UpdateSpeakerUI(_isPlayerSpeaking);

            //Bắt đầu hiện ra nội dung hội thoại
            DisplayCurrentLine();
        }

        /// <summary>
        /// Hàm này giúp đồng bộ state của quest trong NPC này
        /// </summary>
        void SyncQuestState()
        {
            if (_dialogueData._quest == null) return; //Nếu không có quest trong NPC thì return

            string questID = _dialogueData._quest._questID; //Lấy questID của quest

            //Kiểm tra bằng questID và các hàm kiểm tra trong QuestController để thay đổi state
            if(QuestController.Instance.IsQuestCompleted(questID) || 
                QuestController.Instance.IsQuestHandedIn(questID))
            {
                _questState = QuestState.Completed;
            }
            else if(QuestController.Instance.IsQuestActive(questID))
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
                _isPlayerSpeaking = false;
                UpdateSpeakerUI(_isPlayerSpeaking);
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

                string playerLine = null;
                if(choice._playerReplyLines != null && i < choice._playerReplyLines.Length)
                {
                    playerLine = choice._playerReplyLines[i];
                }

                _dialogueUI.CreateChoice(choice._choices[i], () => ChooseOption(nextIndex, giveQuest, playerLine));
            }
        }

        //Hàm này thay đổi _dialogueIndex sau đó xóa lựa chọn và hiện ra đoạn hội thoại với _dialogueIndex mới
        void ChooseOption(int nextIndex, bool giveQuest, string playerText)
        {
            if(giveQuest)
            {
                QuestController.Instance.AcceptQuest(_dialogueData._quest);
                _questState = QuestState.InProgress;
            }

            StopAllCoroutines();
            _dialogueUI.ClearChoice();

            if(string.IsNullOrEmpty(playerText))
            {
                _dialogueIndex = nextIndex;
                _isPlayerSpeaking = false;

                _dialogueUI.SetupInfo(_dialogueData._npcName, _dialogueData._npcPortrait);
                UpdateSpeakerUI(_isPlayerSpeaking);

                DisplayCurrentLine();
                return;
            }

            _isPlayerSpeaking = true;
            UpdateSpeakerUI(_isPlayerSpeaking);

            StartCoroutine(PlayerSpeakThenContinue(playerText, nextIndex));
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

        /// <summary>
        /// Hàm Coroutine player nói chuyện sau khi chọn option
        /// </summary>
        /// <param name="playerText"></param>
        /// <param name="nextIndex"></param>
        /// <returns></returns>
        IEnumerator PlayerSpeakThenContinue(string playerText, int nextIndex)
        {
            _isTyping = true;

            _dialogueUI.SetUpPlayerInfo(_dialogueData._playerName);
            _dialogueUI.SetupDialogueText("");

            foreach(char c in playerText)
            {
                _dialogueUI.SetupDialogueText(_dialogueUI._dialogueText.text += c);
                yield return new WaitForSeconds(_dialogueData._typingSpeed);
            }

            _isTyping = false;

            yield return new WaitForSeconds(_dialogueData._autoProgressDelay);

            _dialogueIndex = nextIndex;
            _isPlayerSpeaking = false;
            _dialogueUI.SetupInfo(_dialogueData._npcName, _dialogueData._npcPortrait);
            UpdateSpeakerUI(_isPlayerSpeaking);

            DisplayCurrentLine();
        }

        void UpdateSpeakerUI(bool isPlayer)
        {
            if(isPlayer)
                _dialogueUI.ShowPlayerSpeaking();
            else
                _dialogueUI.ShowNPCSpeaking();
        }

        /// <summary>
        /// Hàm kết thúc dialogue event
        /// </summary>
        void EndDialogue()
        {
            if(_questState == QuestState.Completed && 
                !QuestController.Instance.IsQuestHandedIn(_dialogueData._quest._questID))
            {
                HandinNPCQuest(_dialogueData._quest);
            }

            StopAllCoroutines();
            _isDialogueActive = false;
            _dialogueUI.SetupDialogueText("");
            _dialogueUI.ShowDialogueUI(false);
            PauseController.SetPaused(false);
            HUDController.Instance.HidePlayerHUD(false);
        }

        //Hàm này giúp trả quest về cho NPC (xóa quest khỏi Quest Log)
        void HandinNPCQuest(Quest quest)
        {
            RewardController.Instance.GiveQuestReward(quest);
            QuestController.Instance.HandinQuest(quest._questID);
        }

        #endregion
    }

    public enum QuestState { NotStarted, InProgress, Completed }
}

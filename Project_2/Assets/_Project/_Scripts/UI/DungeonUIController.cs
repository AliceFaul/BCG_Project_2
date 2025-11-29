using System.Collections;
using _Project._Scripts.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.UI
{
    public class DungeonUIController : MonoBehaviour
    {
        public static DungeonUIController Instance { get; private set; }

        [Header("Tham chiếu các thành phần trong UI")]
        [SerializeField] private GameObject _dungeonPanel;
        [SerializeField] private TMP_Text _dungeonNameText;
        [SerializeField] private TMP_Text _dungeonLevelText;
        [SerializeField] private TMP_Text _dungeonCooldownText;
        [SerializeField] private TMP_Dropdown _difficultDropDown;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        DungeonData _currentData;
        PolygonCollider2D _currentBoundary;
        Difficult _currentDiff;
        Vector3 _currentEntryPoint;
        Coroutine _cooldownRoutine;

        private void Awake()
        {
            if(Instance == null) Instance = this;
            else Destroy(gameObject);

            _dungeonPanel.SetActive(false);

            _difficultDropDown.onValueChanged.AddListener(OnDifficultChanged);
            _confirmButton.onClick.AddListener(OnConfirm);
            _cancelButton.onClick.AddListener(OnCancel);
        }

        public void ShowDungeonConfirmUI(DungeonData data, PolygonCollider2D dungeonBoundary, Vector3 entryPoint)
        {
            HUDController.Instance.HidePlayerHUD(true);

            _currentData = data;
            _currentBoundary = dungeonBoundary;
            _currentEntryPoint = entryPoint;

            _dungeonPanel.SetActive(true);
            _dungeonNameText.text = _currentData._dungeonName;

            _difficultDropDown.value = 0;
            OnDifficultChanged(0);

            if (_cooldownRoutine != null)
                StopCoroutine(_cooldownRoutine);
            _cooldownRoutine = StartCoroutine(UpdateCooldownUI());
        }

        #region Dungeon UI Setting

        void OnDifficultChanged(int index)
        {
            _currentDiff = (Difficult)index;
            int req = _currentData.GetLevelRequirement(_currentDiff);
            _dungeonLevelText.text = $"Level requirement: {req}";
        }

        void OnConfirm()
        {
            if (_currentData.IsOnCooldown()) return;

            DungeonController.Instance.EnterDungeon(_currentData, _currentBoundary, _currentEntryPoint, _currentDiff);
            Hide();
        }

        void OnCancel()
        {
            HUDController.Instance?.HidePlayerHUD(false);
            Hide();
        }

        public void Hide()
        {
            if(_cooldownRoutine != null)
                StopCoroutine(_cooldownRoutine);
            _dungeonPanel.SetActive(false);
        }

        #endregion

        #region DungeonCooldown Coroutine

        IEnumerator UpdateCooldownUI()
        {
            while(CurrentPanelActive())
            {
                if(_currentData.IsOnCooldown())
                {
                    float remaining = _currentData._lastClearTime - Time.time;
                    if(remaining < 0) remaining = 0;

                    int min = Mathf.FloorToInt(remaining / 60);
                    int sec = Mathf.FloorToInt(remaining % 60);

                    if(min > 0)
                        _dungeonCooldownText.text = $"Cooldown: {min:D2}:{sec:D2}";
                    else
                        _dungeonCooldownText.text = $"Cooldown: {sec:D2}";

                    _confirmButton.interactable = false;
                }
                else
                {
                    _dungeonCooldownText.text = "";
                    _confirmButton.interactable = true;
                }

                yield return new WaitForSeconds(1f);
            }
        }

        #endregion

        bool CurrentPanelActive() => _dungeonPanel != null && _dungeonPanel.activeSelf;
    }
}

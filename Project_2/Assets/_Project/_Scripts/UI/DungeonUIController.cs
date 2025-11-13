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
        [SerializeField] private TMP_Dropdown _difficultDropDown;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        DungeonData _currentData;
        PolygonCollider2D _currentBoundary;
        Difficult _currentDiff;
        Vector3 _currentEntryPoint;

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
            DungeonController.Instance.EnterDungeon(_currentData, _currentBoundary, _currentEntryPoint);
            Hide();
        }

        void OnCancel()
        {
            HUDController.Instance?.HidePlayerHUD(false);
            Hide();
        }

        public void Hide() => _dungeonPanel.SetActive(false);

        #endregion
    }
}

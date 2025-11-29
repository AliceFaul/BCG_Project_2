using _Project._Scripts.Core;
using _Project._Scripts.Player;
using _Project._Scripts.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project._Scripts.UI
{
    /// <summary>
    /// Script này chỉ dùng để bật tắt các UI trên HUD bằng input của người chơi
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameObject _menuCanvas;
        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private GameObject _infoMenu;

        [SerializeField] private TMP_Text _coinText;

        SceneLoader _sceneLoader;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _menuCanvas.SetActive(false);
            _sceneLoader = FindAnyObjectByType<SceneLoader>();

            if(PlayerWallet.Instance != null)
                PlayerWallet.Instance.OnCoinChanged += OnCoinChanged;

            PlayerWallet.Instance.RefreshCoinUI();
        }

        // Update is called once per frame
        void Update()
        {
            ActiveMenu();
            ActivePauseMenu();
            ActiveInfoMenu();
        }

        //Hàm bật tắt Menu bằng input của người chơi
        void ActiveMenu()
        {
            if (PlayerInput.Instance == null) return;

            if(PlayerInput.Instance._menuInput)
            {
                if(!_menuCanvas.activeSelf && PauseController.IsGamePaused)
                {
                    return;
                }

                SoundEffectManager.Instance.Play("Menu");
                _menuCanvas.SetActive(!_menuCanvas.activeSelf);
                PauseController.SetPaused(_menuCanvas.activeSelf);
                HUDController.Instance.HidePlayerHUD(_menuCanvas.activeSelf);
            }
        }

        #region Pause Menu Controller

        void ActivePauseMenu()
        {
            if (PlayerInput.Instance == null) return;

            if(PlayerInput.Instance._pauseInput)
            {
                if(!_pauseMenu.activeSelf && PauseController.IsGamePaused)
                    return;

                SoundEffectManager.Instance.Play("Menu");
                _pauseMenu.SetActive(!_pauseMenu.activeSelf);
                Time.timeScale = _pauseMenu.activeSelf ? 0 : 1;
                HUDController.Instance.HidePlayerHUD(_pauseMenu.activeSelf);
            }
        }

        public void ActivePauseMenuByButton()
        {
            if (!_pauseMenu.activeSelf && PauseController.IsGamePaused)
                return;

            SoundEffectManager.Instance.Play("Menu");
            _pauseMenu.SetActive(!_pauseMenu.activeSelf);
            Time.timeScale = _pauseMenu.activeSelf ? 0 : 1;
            HUDController.Instance.HidePlayerHUD(_pauseMenu.activeSelf);
        }

        public async void BackToMainMenu()
        {
            Time.timeScale = 1;

            SaveController.Instance.SaveGame();

            await SceneManager.UnloadSceneAsync("PersistentGameplay");
            await _sceneLoader.LoadingSceneGroup(0);
        }

        public void SaveGameButton() => SaveController.Instance.SaveGame();

        #endregion

        void ActiveInfoMenu()
        {
            if (PlayerInput.Instance == null) return;

            if (PlayerInput.Instance._infoInput)
            {
                if (!_infoMenu.activeSelf && PauseController.IsGamePaused)
                {
                    return;
                }

                SoundEffectManager.Instance.Play("Menu");
                _infoMenu.SetActive(!_infoMenu.activeSelf);
                PauseController.SetPaused(_infoMenu.activeSelf);
                HUDController.Instance.HidePlayerHUD(_infoMenu.activeSelf);
            }
        }

        void UpdateCoinText()
        {
            if (PlayerWallet.Instance == null || _coinText == null) return;

            _coinText.text = PlayerWallet.Instance.Coins.ToString();
        }

        void OnCoinChanged(int x) => UpdateCoinText();
    }
}

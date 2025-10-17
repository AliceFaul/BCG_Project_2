using _Project._Scripts.Core;
using _Project._Scripts.Player;
using UnityEngine;

namespace _Project._Scripts.UI
{
    /// <summary>
    /// Script này chỉ dùng để bật tắt các UI trên HUD bằng input của người chơi
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameObject _menuCanvas;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _menuCanvas.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            ActiveMenu();
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

                SoundEffectManager.Play("Menu");
                _menuCanvas.SetActive(!_menuCanvas.activeSelf);
                PauseController.SetPaused(_menuCanvas.activeSelf);
            }
        }
    }
}

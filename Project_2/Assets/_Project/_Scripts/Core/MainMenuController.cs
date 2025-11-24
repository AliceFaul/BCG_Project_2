using UnityEngine;

namespace _Project._Scripts.Core
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject[] _objectsToHide;

        public void StartGame()
        {
            //Đầu tiên sẽ ẩn hết mọi thứ trong Menu và chuẩn bị load scene
            HideMenu();
            
            SaveController.Instance.ContinueGame();
        }

        void HideMenu()
        {
            for(int i = 0; i < _objectsToHide.Length; i++)
            {
                _objectsToHide[i].SetActive(false);
            }
        }
    }
}

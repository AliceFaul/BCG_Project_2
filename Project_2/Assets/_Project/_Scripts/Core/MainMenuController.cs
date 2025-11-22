using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace _Project._Scripts.Core
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject[] _objectsToHide;

        [Header("Các scene sẽ load trong Main Menu")]
        [SerializeField] private SceneField _persistentScene;
        [SerializeField] private SceneField _startBiomeScene;

        List<AsyncOperation> _sceneToLoads = new List<AsyncOperation>();

        public void StartGame()
        {
            //Đầu tiên sẽ ẩn hết mọi thứ trong Menu và chuẩn bị load scene
            HideMenu();

            _sceneToLoads.Add(SceneManager.LoadSceneAsync(_persistentScene));
            _sceneToLoads.Add(SceneManager.LoadSceneAsync(_startBiomeScene, LoadSceneMode.Additive));
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

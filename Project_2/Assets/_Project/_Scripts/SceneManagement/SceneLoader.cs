using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private Image _loadingImage;
        [SerializeField] private float _fillSpeed;
        [SerializeField] private Canvas _loadingCanvas;
        [SerializeField] private Camera _loadingCamera;
        [SerializeField] private SceneGroup[] _sceneGroups;

        float _targetProgress;
        bool _isLoading;
        
        public readonly SceneGroupController _sceneController = new SceneGroupController();

        private void Awake()
        {
            _sceneController.OnSceneLoaded += sceneName => Debug.Log($"Loaded scene: {sceneName}");
            _sceneController.OnSceneUnLoaded += sceneName => Debug.Log($"Unloaded scene: {sceneName}");
            _sceneController.OnSceneGroupLoaded += () => Debug.Log($"Loaded scene group");
        }

        async void Start()
        {
            await LoadingSceneGroup(0);
        }

        private void Update()
        {
            if (!_isLoading) return;

            float currentFillAmount = _loadingImage.fillAmount;
            float progressDifferent = Mathf.Abs(currentFillAmount - _targetProgress);
            float dynamicSpeed = progressDifferent * _fillSpeed;

            _loadingImage.fillAmount = Mathf.Lerp(currentFillAmount, _targetProgress, Time.deltaTime * dynamicSpeed);
        }

        public async Task LoadingSceneGroup(int index)
        {
            _loadingImage.fillAmount = 0f;
            _targetProgress = 1f;

            if(index < 0 || index >= _sceneGroups.Length)
            {
                Debug.LogError($"Invalid scene group index: {index}");
                return;
            }

            LoadingProgress progress = new LoadingProgress();
            progress._Progressed += target => _targetProgress = Mathf.Max(target, _targetProgress);

            EnableLoadingScreen();
            await _sceneController.LoadScenes(_sceneGroups[index], progress);
            EnableLoadingScreen(false);
        }

        void EnableLoadingScreen(bool enable = true)
        {
            _isLoading = enable;
            _loadingCanvas.gameObject.SetActive(enable);
            _loadingCamera.gameObject.SetActive(enable);
        }
    }

    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> _Progressed;

        const float ratio = 1f;

        public void Report(float value)
        {
            _Progressed?.Invoke(value / ratio);
        }
    }
}

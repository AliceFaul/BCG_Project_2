using System.Collections;
using _Project._Scripts.Core;
using _Project._Scripts.Player;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.UI
{
    public class DeathScreenController : MonoBehaviour
    {
        PlayerHealth _player;
        private const float _minDistance = 1f;
        private const float _maxDistance = 2f;

        [Header("Tham chiếu các object trong UI vào")]
        [SerializeField] private CanvasGroup _deathPanel;
        [SerializeField] private CanvasGroup _deathTextGroup;
        [SerializeField] private CanvasGroup _reviveGroup;
        [SerializeField] private Button _reviveButton;

        [Header("Các biến kiểm soát trong death screen")]
        [SerializeField] private float _fadeDuration = 1.5f;
        [SerializeField] private float _textDelay = .5f;
        [SerializeField] private float _reviveDelay = 2.5f;

        [Header("Các joke text")]
        [SerializeField] private TMP_Text _jokeText;
        [SerializeField] private string[] _joke;

        bool _isShowing = false;

        private void Awake()
        {
            _player = FindAnyObjectByType<PlayerHealth>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _deathPanel.gameObject.SetActive(false);
            _reviveButton.onClick.AddListener(() => RevivePlayer());

            if (_player != null)
                _player.OnDead += ShowDeathScreen;
        }

        public void ShowDeathScreen()
        {
            if(_isShowing) return;
            _isShowing = true;

            _deathPanel.alpha = 0;
            _deathTextGroup.alpha = 0;
            _reviveGroup.alpha = 0;
            SetJokeText();

            _deathPanel.gameObject.SetActive(true);
            StartCoroutine(DeathCoroutineFlow());
            HUDController.Instance.HidePlayerHUD(true);
        }

        void HideDeathScreen()
        {
            _isShowing = false;

            _deathPanel.alpha = 0;
            _deathTextGroup.alpha = 0;
            _reviveGroup.alpha = 0;

            _deathPanel.gameObject.SetActive(false);
            HUDController.Instance.HidePlayerHUD(false);
        }

        public void RevivePlayer()
        {
            HideDeathScreen();
            Vector3 posCheckpoint = CheckpointController.Instance.GetCheckpointPosition();
            Vector2 posOffset = Random.insideUnitCircle.normalized * Random.Range(_minDistance, _maxDistance);
            Vector2 pos = (Vector2)posCheckpoint + posOffset;

            if (_player != null)
            {
                _player.Revive();
                _player.transform.position = pos;
                _player.gameObject.SetActive(true);
            }

            FindAnyObjectByType<CinemachineConfiner2D>().BoundingShape2D = CheckpointController.Instance.GetMapBoundary();
        }

        void SetJokeText()
        {
            int index = Random.Range(0, _joke.Length);

            _jokeText.text = _joke[index];
        }

        #region Coroutine Death Screen

        IEnumerator DeathCoroutineFlow()
        {
            yield return StartCoroutine(FadeCanvas(_deathPanel, 0f, 1f, _fadeDuration));

            yield return new WaitForSeconds(_textDelay);
            yield return StartCoroutine(FadeCanvas(_deathTextGroup, 0f, 1f, _fadeDuration));

            yield return new WaitForSeconds(_reviveDelay);
            yield return StartCoroutine(FadeCanvas(_reviveGroup, 0f, 1f, _fadeDuration));
        }

        IEnumerator FadeCanvas(CanvasGroup obj, float from, float to, float duration)
        {
            float t = 0f;
            obj.alpha = from;
            while(t < duration)
            {
                t += Time.deltaTime;
                obj.alpha = Mathf.Lerp(from, to, t / duration);
                yield return null;
            }

            obj.alpha = to;
        }

        #endregion
    }
}

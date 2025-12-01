using System;
using System.Collections;
using System.Threading.Tasks;
using _Project._Scripts.Core;
using Unity.Cinemachine;
using UnityEngine;

namespace _Project._Scripts.UI
{
    public class FadeTransition : MonoBehaviour
    {
        public static FadeTransition Instance { get; private set; }

        [SerializeField] private CanvasGroup _fadePanel;
        [SerializeField] private float _fadeDuration = .5f;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            _fadePanel.alpha = 0;
        }

        public void FadeOut(Action onFadeComplete = null)
        {
            StartCoroutine(FadeRoutine(1, onFadeComplete));
        }

        public void FadeIn(Action onFadeComplete = null)
        {
            StartCoroutine(FadeRoutine(0, onFadeComplete));
        }

        IEnumerator FadeRoutine(float targetAlpha, Action onComplete)
        {
            float startAlpha = _fadePanel.alpha;
            float t = 0f;

            while (t < _fadeDuration)
            {
                t += Time.deltaTime;
                _fadePanel.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / _fadeDuration);
                yield return null;
            }

            _fadePanel.alpha = targetAlpha;
            onComplete?.Invoke();
        }

        //async Task Fade(float targetTransparency)
        //{
        //    float start = _fadePanel.alpha, t = 0f;

        //    while(t < _fadeDuration)
        //    {
        //        t += Time.deltaTime;
        //        _fadePanel.alpha = Mathf.Lerp(start, targetTransparency, t / _fadeDuration);
        //        await Task.Yield();
        //    }

        //    _fadePanel.alpha = targetTransparency;
        //}

        public IEnumerator FadeMapTransition(Action duringFade = null)
        {
            PauseController.SetPaused(true);
            yield return FadeRoutine(1, null);

            duringFade?.Invoke();
            yield return new WaitForSeconds(.3f);

            yield return FadeRoutine(0, null);
            PauseController.SetPaused(false);
        }

        //public async Task FadeOut()
        //{
        //    await Fade(1);
        //}

        //public async Task FadeIn()
        //{
        //    await Fade(0);
        //}

        public IEnumerator FadeOutIn(Action duringFade = null)
        {
            yield return FadeRoutine(1, null);
            duringFade?.Invoke();

            CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
            Transform cam = Camera.main.transform;
            Transform player = GameObject.FindWithTag("Player").transform;

            float time = 0f;
            while ((brain.IsBlending || Vector3.Distance(cam.position, player.position) > .5f) &&
                time < 1.5f)
            {
                time += Time.deltaTime;
                yield return null;
            }

            yield return FadeRoutine(0, null);
        }
    }
}

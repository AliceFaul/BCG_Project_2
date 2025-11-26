using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project._Scripts.Core
{
    [RequireComponent(typeof(AudioSource))]
    public class BGMController : PersistentSingleton<BGMController>
    {
        AudioSource _source;
        Slider _bgmSlider;

        [SerializeField] private AudioClip _bgmNormalClips;
        [SerializeField] private AudioClip _bgmCombatClips;
        private const string bgmVolumeKey = "BGMVolume";

        Coroutine _fadeRoutine;
        MusicMode _mode;
        float _savedVolume;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _source = GetComponent<AudioSource>();
            SceneManager.sceneLoaded += OnSceneLoaded;

            _savedVolume = PlayerPrefs.GetFloat(bgmVolumeKey, 1f);

            if(_bgmNormalClips)
            {
                _source.volume = _savedVolume;
                PlayBGM(false, _bgmNormalClips);
            }
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(!_bgmSlider)
            {
                var slider = GameObject.Find("MusicSlider").GetComponent<Slider>();
                
                if(slider)
                {
                    _bgmSlider = slider;
                    _bgmSlider.value = _savedVolume;

                    _bgmSlider.onValueChanged.RemoveAllListeners();
                    _bgmSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
                }
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void PlayBGM(bool resetSong, AudioClip clip = null)
        {
            if(clip != null)
            {
                _source.clip = clip;
            }
            else if(_source.clip != null)
            {
                if(resetSong)
                    _source.Stop();

                _source.Play();
            }
        }

        public void PauseBGM() => _source.Stop();

        public void ChangeMusicMode(MusicMode mode)
        {
            _mode = mode;

            AudioClip clip = (_mode == MusicMode.Normal) ? _bgmNormalClips : _bgmCombatClips;

            if (clip == null)
                return;

            SwitchBGM(clip);
        }

        void SwitchBGM(AudioClip clip)
        {
            if (_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);

            _fadeRoutine = StartCoroutine(CrossFade(clip));
        }

        IEnumerator CrossFade(AudioClip newClip)
        {
            float duration = .8f;
            float startVolume = _source.volume;

            for(float i = 0; i < duration; i += Time.deltaTime)
            {
                _source.volume = Mathf.Lerp(startVolume, 0, i / duration);
                yield return null;
            }

            PlayBGM(false, newClip);

            for(float i = 0; i < duration; i += Time.deltaTime)
            {
                _source.volume = Mathf.Lerp(0, startVolume, i / duration);
                yield return null;
            }

            _source.volume = startVolume;
        }

        #region Music Setting

        public void SetVolume(float volume)
        {
            _savedVolume = volume;

            if(_fadeRoutine == null)
                _source.volume = volume;

            PlayerPrefs.SetFloat(bgmVolumeKey, volume);
            PlayerPrefs.Save();
        }

        public void OnValueChanged()
        {
            if (!_bgmSlider) return;

            SetVolume(_bgmSlider.value);
        }

        void LoadBGMVolume()
        {
            if (!_source) return;

            _source.volume = PlayerPrefs.GetFloat(bgmVolumeKey, 1f);
            _bgmSlider.value = PlayerPrefs.GetFloat(bgmVolumeKey, 1f);
            _bgmSlider.onValueChanged.RemoveAllListeners();
            _bgmSlider.onValueChanged.AddListener(SetVolume);
        }

        #endregion
    }

    public enum MusicMode { Normal, Battle }
}

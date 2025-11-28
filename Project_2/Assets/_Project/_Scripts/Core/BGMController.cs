using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.Core
{
    [RequireComponent(typeof(AudioSource))]
    public class BGMController : MonoBehaviour
    {
        public static BGMController Instance { get; private set; }

        AudioSource _source;
        [SerializeField] Slider _bgmSlider;

        [SerializeField] private AudioClip _bgmNormalClips;
        [SerializeField] private AudioClip _bgmCombatClips;
        private const string bgmVolumeKey = "BGMVolume";

        Coroutine _fadeRoutine;
        MusicMode _mode;
        float _savedVolume;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
                Destroy(gameObject);

            _source = GetComponent<AudioSource>();

            LoadBGMVolume();

            //_sceneLoader = FindAnyObjectByType<SceneLoader>();

            //if (_sceneLoader != null)
            //{
            //    _sceneLoader._sceneController.OnSceneLoaded += OnSceneLoaded;
            //    Debug.Log("BGMController has subscribe OnSceneGroupLoaded");
            //}
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _savedVolume = PlayerPrefs.GetFloat(bgmVolumeKey, 1f);

            if(_bgmNormalClips != null)
            {
                _source.volume = _savedVolume;
                PlayBGM(false, _bgmNormalClips);
            }

            _bgmSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
        }

        //void OnSceneLoaded(string sceneName)
        //{
        //    if(_bgmSlider == null)
        //    {
        //        if (sceneName == "MainMenu" || sceneName == "PersistentGameplay")
        //        {
        //            GameObject slider = GameObject.Find("MusicSlider");

        //            if (slider != null)
        //            {
        //                _bgmSlider = slider.GetComponent<Slider>();
        //                _bgmSlider.value = _savedVolume;

        //                _bgmSlider.onValueChanged.RemoveAllListeners();
        //                _bgmSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
        //            }
        //            else
        //            {
        //                Debug.LogWarning("BGM Slider not found");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("BGM Slider has reference");
        //    }
        //}

        public void PlayBGM(bool resetSong, AudioClip clip = null)
        {
            if(clip != null)
            {
                _source.clip = clip;
                _source.Play();
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

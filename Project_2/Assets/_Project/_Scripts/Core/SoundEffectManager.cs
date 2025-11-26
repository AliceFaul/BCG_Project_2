using UnityEngine;
using UnityEngine.UI;

namespace _Project._Scripts.Core
{
    /// <summary>
    /// Script này sẽ chạy hết các AudioClip Sound Effect trong game
    /// </summary>
    [RequireComponent(typeof(AudioSource), typeof(SoundEffectLibrary))]
    public class SoundEffectManager : MonoBehaviour
    {
        public static SoundEffectManager Instance { get; private set; }

        //Thiết lập các component cần thiết
        private static AudioSource _sfSource; //AudioSource của các âm thanh không thay đổi tần số
        private static AudioSource _randomPitchAudioSource; //AudioSource của các âm thanh sẽ random tần số như Footstep,...
        private static SoundEffectLibrary _dictionary;

        [SerializeField] private Slider _sfSlider;
        private const string SfxVolumeKey = "SfxVolume";
        private const string RandomPitchVolumeKey = "RandomPitchVolume";

        private void Awake()
        {
            if(Instance == null) Instance = this;
            else Destroy(gameObject);

            AudioSource[] audioSources = GetComponents<AudioSource>();
            _sfSource = audioSources[0];
            _randomPitchAudioSource = audioSources[1];

            _dictionary = GetComponent<SoundEffectLibrary>();

            //Tải âm thanh đã lưu
            LoadSfxVolume();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //Thay đổi âm thanh bằng slider
            if (_sfSlider == null) return;

            _sfSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
        }

        public static void Play(string name, bool randomPitch = false) //Nếu muốn sử dụng source randomPitch thì sẽ set true nhưng mặc định là false
        {
            if (_dictionary == null || _sfSource == null) return;

            //Lấy audioclip từ dictionary bằng hàm GetAudioClip
            AudioClip audioClip = _dictionary.GetAudioClip(name);
            if(audioClip != null)
            {
                //Kiểm tra sẽ sử dụng Source nào 
                if (randomPitch)
                {
                    _randomPitchAudioSource.pitch = Random.Range(1f, 1.5f);
                    _randomPitchAudioSource.PlayOneShot(audioClip);
                }
                else
                {
                    _sfSource.PlayOneShot(audioClip);
                }
            }
        }

        public static void SetVolume(float volume)
        {
            _sfSource.volume = volume;
            _randomPitchAudioSource.volume = volume;
            PlayerPrefs.SetFloat(SfxVolumeKey, volume); //Lưu giá trị âm thanh lại
            PlayerPrefs.SetFloat(RandomPitchVolumeKey, volume);
            PlayerPrefs.Save();
        }

        public void OnValueChanged()
        {
            if(_sfSlider == null) return;

            SetVolume(_sfSlider.value);
        }

        //Hàm tải âm thanh qua PlayerPrefs
        private void LoadSfxVolume()
        {
            if(_sfSource == null) return;

            //Nếu không có lưu giá trị nào trong PlayerPrefs thì sẽ mặc định là max volume
            _sfSource.volume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
            _randomPitchAudioSource.volume = PlayerPrefs.GetFloat(RandomPitchVolumeKey, 1f);
            _sfSlider.value = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
            _sfSlider.value = PlayerPrefs.GetFloat(RandomPitchVolumeKey, 1f);
        }
    }
}

using UnityEngine;
using System.Collections.Generic;

namespace _Project._Scripts.Core
{
    /// <summary>
    /// Đây là script dùng để chứa hết các SFX trong game 
    /// </summary>
    public class SoundEffectLibrary : MonoBehaviour
    {
        [SerializeField] private SoundEffect[] _soundEffectGroups;
        private Dictionary<string, List<AudioClip>> _soundDictionary;

        private void Awake()
        {
            Initialize();
        }

        //Hàm khởi tạo lưu các audioclip trong soundEffectGroup vào soundDictionary
        void Initialize()
        {
            _soundDictionary = new Dictionary<string, List<AudioClip>>();
            foreach(SoundEffect soundEffect in _soundEffectGroups)
            {
                _soundDictionary[soundEffect._name] = soundEffect._audioClips;
            }
        }

        //Hàm lấy audioclip bằng name trong dictionary ra để sử dụng trong manager của sound effect
        public AudioClip GetAudioClip(string name)
        {
            if(_soundDictionary.ContainsKey(name))
            {
                //Nếu trùng tên với các audioclip khác thì cứ Random ra, còn không thì vẫn cho ra một audioclip
                List<AudioClip> audioClips = _soundDictionary[name];
                if(audioClips.Count > 0)
                {
                    return audioClips[Random.Range(0, audioClips.Count)];
                }
            }
            return null;
        }
    }

    //Tạo một struct lưu tên và audioclip của một âm thanh 
    [System.Serializable]
    public struct SoundEffect
    {
        public string _name;
        public List<AudioClip> _audioClips;
    }
}

using UnityEngine;

namespace _Project._Scripts.Core
{
    public class SpriteShaderController : MonoBehaviour
    {
        private SpriteRenderer _sr;
        private MaterialPropertyBlock _mpd;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _mpd = new MaterialPropertyBlock();
        }

        public void SetDissolve(float value)
        {
            _sr.GetPropertyBlock(_mpd);
            _mpd.SetFloat("_Fade", value);
            _sr.SetPropertyBlock(_mpd);
        }

        public void SetDamageFlash(float value)
        {
            _sr.GetPropertyBlock(_mpd);
            _mpd.SetFloat("_FlashAmount", value);
            _sr.SetPropertyBlock(_mpd);
        }
    }
}


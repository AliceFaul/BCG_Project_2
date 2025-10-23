using System;
using System.Collections;
using _Project._Scripts.Core;
using _Project._Scripts.UI;
using UnityEngine;

namespace _Project._Scripts.Player
{
    [RequireComponent(typeof(PlayerMovement), typeof(Material))]
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        public event Action OnDead;
        [SerializeField] private Material _objectDissolve, _damageFlash;

        [Header("Các thông số máu")]
        [SerializeField] private float _maxHealth = 100f; //Máu tối đa của người chơi
        [SerializeField] private float _currentHealth; //Máu hiện tại của người chơi

        [Space(10)]

        [Header("Các thông số khi trúng đòn và chết")]
        //Setup hiệu ứng khi chết
        private float _fade = 1f;
        [Tooltip("Màu sắc khi chết")]
        [SerializeField] private Color _deadColor = Color.black; //Khi chết, người chơi sẽ biến thành màu đen

        //Setup hiệu ứng khi trúng đòn đánh
        [ColorUsage(true, true)]
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private float _flashTime = 0.25f;
        [SerializeField] private AnimationCurve _flashSpeedCurve;
        [SerializeField] private ParticleSystem _damageParticle;

        #region Heath Unity Life Cycle

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _currentHealth = _maxHealth;
            HUDController.Instance.UpdateHealthUI(_currentHealth, _maxHealth);
        }

        //Hàm trừ máu, nếu damage là âm sẽ mất máu còn dương sẽ hồi máu
        public void TakeDamage(float damage)
        {
            _currentHealth += damage;
            DamageParticle();
            SoundEffectManager.Play("Hit");
            HUDController.Instance.UpdateHealthUI(_currentHealth, _maxHealth);

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                OnDead.Invoke();
                Debug.LogWarning("Player has dead and OnDead has Invoke");
                gameObject.GetComponent<SpriteRenderer>().color = _deadColor;
                StartCoroutine(Dissolve(gameObject, _fade));
            }

            StartCoroutine(DamageFlasher());
        }

        void DamageParticle()
        {
            if (_damageParticle == null) return;

            ParticleSystem damageParti = Instantiate(_damageParticle, transform.position, Quaternion.identity);
        }

        #endregion

        #region Dead Effect
        //Hàm Coroutine để sử dụng hiệu ứng dissolve và disable player
        IEnumerator Dissolve(GameObject player, float time)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            SpriteShaderController shaderController = GetComponent<SpriteShaderController>();

            sr.material = _objectDissolve;
            yield return new WaitForSeconds(2f);

            float t = 1f;

            while (t > 0f)
            {
                t -= Time.deltaTime / time;
                shaderController.SetDissolve(t);
                yield return null;
            }

            player.SetActive(false);
        }
        #endregion

        #region Hit Effect
        //Hàm Coroutine để sử dụng hiệu ứng trúng đòn
        IEnumerator DamageFlasher()
        {
            SpriteShaderController shaderController = GetComponent<SpriteShaderController>();

            //Chỉnh màu
            SetFlashColor();

            //Nội suy hiệu ứng
            float currentFlashAmount = 0f;
            float elapsedTime = 0f;

            while (elapsedTime < _flashTime)
            {
                elapsedTime += Time.deltaTime;
                currentFlashAmount = Mathf.Lerp(1f, _flashSpeedCurve.Evaluate(elapsedTime), (elapsedTime / _flashTime));
                shaderController.SetDamageFlash(currentFlashAmount);
                yield return null;
            }
        }

        //Hàm chỉnh màu cho hiệu ứng
        void SetFlashColor()
        {
            _damageFlash.SetColor("_FlashColor", _flashColor);
        }

        #endregion
    }
}

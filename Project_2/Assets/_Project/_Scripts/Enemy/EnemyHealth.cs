using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using _Project._Scripts.Core;
using System;

namespace _Project._Scripts.Enemies
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        //Biến event gửi tín hiệu qua movement để ngừng di chuyển
        public event Action OnDead;

        [SerializeField] private Material _objectDissolve, _damageFlash;

        [SerializeField] private float _maxHealth = 100f; //Lượng máu tối đa
        private float _currentHealth; //Lượng máu hiện tại
        [SerializeField] private Image _healthBar; //Health bar của gameobject

        [Header("Các thông số khi trúng đòn và chết")]
        //Setup hiệu ứng khi chết
        private float _fade = 1f;
        [Tooltip("Màu sắc khi chết")]
        [SerializeField] private Color _deadColor = Color.black;

        //Setup hiệu ứng khi trúng đòn đánh
        [Tooltip("Chỉnh màu damage flash")]
        [ColorUsage(true, true)]
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private float _flashTime = 0.25f;
        [SerializeField] private AnimationCurve _flashSpeedCurve;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _currentHealth = _maxHealth;
            UpdateHealthBar();
        }

        //Implement từ interface IDamageable để chịu sát thương
        public void TakeDamage(float damage)
        {
            _currentHealth += damage;
            _currentHealth = Mathf.Max(_currentHealth, 0);
            UpdateHealthBar();
            if(_currentHealth <= 0)
            {
                _currentHealth = 0;
                OnDead?.Invoke();
                gameObject.GetComponent<SpriteRenderer>().color = _deadColor;
                StartCoroutine(Die(gameObject, _fade));
            }

            StartCoroutine(DamageFlash());
        }

        //Hàm cập nhật thanh máu
        void UpdateHealthBar()
        {
            if (_healthBar == null) return;

            _healthBar.fillAmount = _currentHealth / _maxHealth;
        }

        //Tạch
        IEnumerator Die(GameObject obj, float time)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            SpriteShaderController shaderController = GetComponent<SpriteShaderController>();

            sr.material = _objectDissolve;
            Debug.Log("Get Dissolve Material");
            yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 4f));

            float t = 1f;

            while (t > 0f)
            {
                t -= Time.deltaTime / time;
                shaderController.SetDissolve(t);
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
            obj.SetActive(false);
        }

        //Hàm Coroutine để sử dụng hiệu ứng Damage Flash
        IEnumerator DamageFlash()
        {
            SpriteShaderController shaderController = GetComponent<SpriteShaderController>();
            //Chỉnh màu
            _damageFlash.SetColor("_FlashColor", _flashColor);

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
    }
}

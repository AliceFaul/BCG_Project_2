using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using _Project._Scripts.Core;
using System;
using _Project._Scripts.UI;
using System.Net;

namespace _Project._Scripts.Enemies
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        private EnemyPool _pool; // Tham chiếu pool sinh ra enemy này

        //Biến event gửi tín hiệu qua movement để ngừng di chuyển
        public event Action OnDead;
        public event Action OnRevive;

        public static event Action<string> OnEnemyDefeated;

        EnemyInfo _info;

        [SerializeField] private Material _objectDissolve, _damageFlash;

        [SerializeField] public float _maxHealth = 100f; //Lượng máu tối đa
        public float _currentHealth; //Lượng máu hiện tại
        [SerializeField] private Image _healthBar; //Health bar của gameobject

        [Header("Các thông số khi trúng đòn và chết")]
        //Setup hiệu ứng khi chết
        private float _fade = 1f;
        [Tooltip("Màu sắc khi chết")]
        [SerializeField] private Color _deadColor = Color.black;
        [SerializeField] private int _enemyExperience = 5;

        //Setup hiệu ứng khi trúng đòn đánh
        [Tooltip("Chỉnh màu damage flash")]
        [ColorUsage(true, true)]
        [SerializeField] private Color _flashColor = Color.white;
        [SerializeField] private float _flashTime = 0.25f;
        [SerializeField] private AnimationCurve _flashSpeedCurve;
        [Tooltip("Hiệu ứng particle phát ra khi bị player tấn công")]
        [SerializeField] private ParticleSystem _damageParticle;

        private bool _isDead = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _info = GetComponent<EnemyInfo>();

            if (_info != null)
            {
                if (gameObject.GetComponent<IDungeonEnemy>() == null)
                {
                    EnemyStats stats = _info._enemyData.GetStatsAfterGrowth(HUDController.Instance._currentLevel);
                    SetMaxHealth(stats);
                }
            }

            _currentHealth = _maxHealth;
            UpdateHealthBar();
        }

        private void OnEnable()
        {
            _isDead = false;
            GetComponent<SpriteRenderer>().material = _damageFlash;
            GetComponent<SpriteRenderer>().color = Color.white;

            if (_info != null)
            {
                if (gameObject.GetComponent<IDungeonEnemy>() == null)
                {
                    EnemyStats stats = _info._enemyData.GetStatsAfterGrowth(HUDController.Instance._currentLevel);
                    SetMaxHealth(stats);
                }
            }

            _currentHealth = _maxHealth;
            UpdateHealthBar();
        }

        public void SetMaxHealth(EnemyStats stats)
        {
            _maxHealth = stats._enemyHP;
            _enemyExperience = stats._enemyExperience;

            _currentHealth = _maxHealth;
            UpdateHealthBar();
        }

        public void ReviveInDungeon()
        {
            _isDead = false;
            gameObject.SetActive(true);
            GetComponent<SpriteRenderer>().material = _damageFlash;
            GetComponent<SpriteRenderer>().color = Color.white;

            _currentHealth = _maxHealth;
            UpdateHealthBar();

            OnRevive?.Invoke();
        }

        #region Take Damage, Update Health Bar Life Cycle

        //Implement từ interface IDamageable để chịu sát thương
        public void TakeDamage(float damage)
        {
            if(_isDead) return;

            _currentHealth += damage;
            _currentHealth = Mathf.Max(_currentHealth, 0);
            SoundEffectManager.Instance.Play("Hit");
            DamageParticle();
            UpdateHealthBar();
            if(_currentHealth <= 0)
            {
                _isDead = true;
                _currentHealth = 0;

                OnDead?.Invoke();
                if(gameObject.activeSelf)
                    OnEnemyDefeated?.Invoke(_info._enemyData._enemyID);

                HUDController.Instance.AddExperience(_enemyExperience);
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

        void DamageParticle()
        {
            if(_damageParticle == null) return;

            ParticleSystem damageParti = Instantiate(_damageParticle, transform.position, Quaternion.identity);
            Debug.Log("Spawn particle");
        }

        #endregion

        #region Coroutine Dead, Hit

        //Tạch
        IEnumerator Die(GameObject obj, float time)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            SpriteShaderController shaderController = GetComponent<SpriteShaderController>();

            sr.material = _objectDissolve;
            Debug.Log("Get Dissolve Material");
            yield return new WaitForSeconds(2f);

            float t = 1f;

            while (t > 0f)
            {
                t -= Time.deltaTime / time;
                shaderController.SetDissolve(t);
                yield return null;
            }

            yield return new WaitForSeconds(0.15f);

            if(obj.GetComponent<IDungeonEnemy>() != null)
            {
                obj.SetActive(false);
            }
            else
            {
                _pool.ReturnToPool(obj);
            }
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

        #endregion
        public void Initialize(EnemyPool pool)
        {
            _pool = pool;                  // Gán pool
            _isDead = false;               // Reset trạng thái chết
            _currentHealth = _maxHealth;   // Reset lại máu
            UpdateHealthBar();             // Cập nhật thanh máu
            gameObject.GetComponent<SpriteRenderer>().color = Color.white; // Reset lại màu enemy
        }

    }
}

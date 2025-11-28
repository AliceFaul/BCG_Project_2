using UnityEngine;

namespace _Project._Scripts.Gameplay
{
    public class FogProjectile : MonoBehaviour
    {
        [Header("Fog Setting")]
        [SerializeField] private float _spawnRadius = 3f;
        [SerializeField] private float _moveSpeed = .5f;
        [SerializeField] private float _moveRange = 1.5f;

        private Transform _player;
        private FogInstance[] _fogs;

        public void Init(Transform player, GameObject prefab, int amount)
        {
            if (player == null)
            {
                Debug.LogError("FogProjectile.Init ERROR: player == null");
                return;
            }

            if (prefab == null)
            {
                Debug.LogError("FogProjectile.Init ERROR: prefab == null");
                return;
            }

            _player = player;
            _fogs = new FogInstance[amount];

            for (int i = 0; i < amount; i++)
            {
                Vector2 randomPos = (Vector2)_player.position + Random.insideUnitCircle * _spawnRadius;

                GameObject go = Instantiate(prefab, randomPos, Quaternion.identity);

                _fogs[i] = new FogInstance
                {
                    _go = go,
                    _startPos = go.transform.position,
                    dir = Random.value > 0.5f ? 1 : -1
                };
            }
        }

        private void Update()
        {
            if (_fogs == null)
            {
                Debug.LogError("FogProjectile ERROR: _fogs NULL — Init() chưa được gọi!");
                return;
            }

            if (_player == null)
            {
                Debug.LogError("FogProjectile ERROR: player NULL!");
                return;
            }

            foreach (var fog in _fogs)
            {
                if (fog == null || fog._go == null)
                    continue;

                Vector3 pos = fog._go.transform.position;

                // Horizontal movement
                pos.x += fog.dir * _moveSpeed * Time.deltaTime;

                if (Mathf.Abs(pos.x - fog._startPos.x) > _moveRange)
                    fog.dir *= -1;

                // Lock fog Y to player's Y
                pos.y = _player.position.y;

                fog._go.transform.position = pos;
            }
        }
    }

    class FogInstance
    {
        public GameObject _go;
        public Vector3 _startPos;
        public int dir;
    }


}

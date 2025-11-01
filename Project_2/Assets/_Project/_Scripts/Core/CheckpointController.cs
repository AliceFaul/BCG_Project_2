using UnityEngine;
using _Project._Scripts.Gameplay;

namespace _Project._Scripts.Core
{
    public class CheckpointController : MonoBehaviour
    {
        public static CheckpointController Instance { get; private set; }

        private Checkpoint _currentCheckpoint; 

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void ActivateCheckpoint(Checkpoint checkpoint)
        {
            if (_currentCheckpoint != null)
                _currentCheckpoint.SetActivateCheckpoint(false);

            checkpoint.SetActivateCheckpoint(true);
            _currentCheckpoint = checkpoint;
        }

        public Vector3 GetCheckpointPosition() => _currentCheckpoint != null ? _currentCheckpoint.transform.position : Vector3.zero;

        public PolygonCollider2D GetMapBoundary() => _currentCheckpoint != null ? _currentCheckpoint._mapBoundary : null;
    }
}

using _Project._Scripts.Core;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace _Project._Scripts.Gameplay
{
    public class Checkpoint : MonoBehaviour, IInteractable
    {
        public string _CheckpointID { get; private set; }
        public bool _IsActivated { get; private set; }

        [Header("Property của một checkpoint")]
        [SerializeField] private ParticleSystem _smokeFX;
        [SerializeField] private GameObject _flame;
        [SerializeField] private Light2D _flameLight;
        public PolygonCollider2D _mapBoundary = null;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _CheckpointID ??= GlobalHelper.GenerateChestUniqueID(gameObject);

            if (_smokeFX != null && _flame != null)
            {
                _smokeFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                _flame.SetActive(false);
                _flameLight.intensity = 0f;
            }
            else
                Debug.LogWarning("Missing reference of checkpoint object");
        }

        public bool CanInteract()
        {
            return !_IsActivated;
        }

        public void Interact()
        {
            if (!CanInteract()) return;

            ActivateCheckpoint();
        }

        public void ActivateCheckpoint()
        {
            SetActivateCheckpoint(true);

            CheckpointController.Instance.ActivateCheckpoint(this);
            Debug.Log("Yay! Checkpoint");
        }

        public void SetActivateCheckpoint(bool activated)
        {
            _IsActivated = activated;

            if(_IsActivated)
            {
                if(_flame != null && _smokeFX != null)
                {
                    _flame.SetActive(true);
                    _smokeFX.Play();
                    _flameLight.intensity = 1f;
                }
            }
            else
            {
                if (_flame != null && _smokeFX != null)
                {
                    _flame.SetActive(false);
                    _smokeFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    _flameLight.intensity = 0f;
                }
            }
        }
    }
}

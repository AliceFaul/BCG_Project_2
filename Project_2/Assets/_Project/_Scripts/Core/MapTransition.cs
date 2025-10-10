using Unity.Cinemachine;
using UnityEngine;

namespace _Project._Scripts.Core
{
    public class MapTransition : MonoBehaviour
    {
        [Header("Tham chiếu các thành phần cần thiết cho việc transition")]
        [Tooltip("Boundary của map sẽ transition")]
        [SerializeField] PolygonCollider2D _mapBoundary; //Boundary của map sẽ transition
        CinemachineConfiner2D _cmCam; //Tham chiếu Cinemachine Confiner để đổi boundary

        private void Awake()
        {
            _cmCam = GetComponent<CinemachineConfiner2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                
            }
        }
    }
}

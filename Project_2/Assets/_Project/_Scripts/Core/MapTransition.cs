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
        [SerializeField] private TransitionDirection _dir; //Hướng khi transition camera
        [SerializeField] private float _additivePos = 2f;

        private void Awake()
        {
            _cmCam = FindAnyObjectByType<CinemachineConfiner2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Return nếu không có Cinemachine Confiner để đổi boundary 
            if(_cmCam == null) return;

            if(collision.CompareTag("Player"))
            {
                _cmCam.BoundingShape2D = _mapBoundary;
                MovePlayerPosition(collision.gameObject);
            }
        }

        //Hàm giúp cho việc transition mượt mà hơn
        void MovePlayerPosition(GameObject player)
        {
            //Tạo một biến lấy vị trí hiện tại của player
            Vector3 newPos = player.transform.position;

            //Dựa vào hướng đã setup trong unity sẽ thay đổi vị trị của player
            switch(_dir)
            {
                case TransitionDirection.Up: 
                    newPos.y += _additivePos;
                    break;
                case TransitionDirection.Down:
                    newPos.y -= _additivePos;
                    break;
                case TransitionDirection.Left:
                    newPos.x -= _additivePos;
                    break;
                case TransitionDirection.Right:
                    newPos.x += _additivePos;
                    break;
            }

            //Gán lại vị trí mới cho player
            player.transform.position = newPos;
        }
    }

    public enum TransitionDirection { Up, Down, Left, Right }
}

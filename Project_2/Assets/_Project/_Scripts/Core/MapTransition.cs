using System;
using _Project._Scripts.SceneManagement;
using _Project._Scripts.UI;
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
        [SerializeField] private TransitionMode _mode;
        [SerializeField] private float _additivePos = 2f;
        [SerializeField] private string _boundaryNextSceneName = null;
        [SerializeField] private string _nextSceneGroupName = null;

        SceneLoader _sceneLoader;
        Action _onSceneLoadedDelegate;

        private void Start()
        {
            _cmCam = FindAnyObjectByType<CinemachineConfiner2D>();
            _sceneLoader = FindAnyObjectByType<SceneLoader>();
        }

        private async void OnTriggerEnter2D(Collider2D collision)
        {
            //Return nếu không có Cinemachine Confiner để đổi boundary 
            if(_cmCam == null) return;

            if(collision.CompareTag("Player"))
            {
                if(_mode == TransitionMode.BoundaryOnly)
                {
                    StartCoroutine(FadeTransition.Instance.FadeMapTransition(() =>
                    {
                        _cmCam.BoundingShape2D = _mapBoundary;
                        MovePlayerPosition(collision.gameObject);
                    }));
                }
                else if(_mode == TransitionMode.SceneAndBoundary)
                {
                    if (_boundaryNextSceneName == null || _nextSceneGroupName == null) return;

                    MovePlayerPosition(collision.gameObject);

                    _onSceneLoadedDelegate = () => SetBoundaryInNextScene(_boundaryNextSceneName);

                    _sceneLoader._sceneController.OnSceneGroupLoaded += _onSceneLoadedDelegate;

                    int index = _sceneLoader.GetSceneIndexByName(_nextSceneGroupName);
                    await _sceneLoader.LoadingSceneGroup(index);
                }
            }
        }

        void SetBoundaryInNextScene(string name)
        {
            if(_cmCam == null) return;

            _cmCam.BoundingShape2D = GameObject.Find(name)?.GetComponent<PolygonCollider2D>();

            _sceneLoader._sceneController.OnSceneGroupLoaded -= _onSceneLoadedDelegate;
            _onSceneLoadedDelegate = null;
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

    public enum TransitionMode { BoundaryOnly, SceneAndBoundary }

    public enum TransitionDirection { Up, Down, Left, Right }
}

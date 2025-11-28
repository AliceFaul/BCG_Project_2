using System.Collections;
using _Project._Scripts.Core;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    Animator _anim = null; //Animation của object nếu có

    [Header("Thiết lập các property")]
    [Tooltip("Là parent của các waypoint")]
    [SerializeField] private Transform _waypointParent;


    [Tooltip("Tốc độ của object đang sử dụng script này")]
    [SerializeField] private float _moveSpeed;

    [Tooltip("Thời gian chờ khi đã đi đến waypoint")]
    [SerializeField] private float _waitTime = 2f;

    [Tooltip("Tick sẽ khiến cho object di chuyển tuần hoàn các Waypoint")]
    [SerializeField] private bool _loopingWaypoints = true;

    private Transform[] _waypoints; //Mảng chứa các waypoint nằm trong waypoint parent
    private int _currentWaypointIndex; //Sử dụng index để xác nhận waypoint mà object sẽ đến 
    private bool _isWaiting; //Biến xác nhận đang ngồi chờ

    private float _lastHorizontal;
    private float _lastVertical;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _anim = GetComponent<Animator>();
        //Thêm các child của waypointParent vào mảng
        _waypoints = new Transform[_waypointParent.childCount];

        for(int i = 0; i < _waypointParent.childCount; i++)
        {
            _waypoints[i] = _waypointParent.GetChild(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseController.IsGamePaused || _isWaiting) //Nếu đang pause game và đang chờ thì sẽ đứng yên
        {
            if(_anim != null)
            {
                _anim.SetBool("isWalking", false);
                _anim.SetFloat("LastHorizontal", _lastHorizontal);
                _anim.SetFloat("LastVertical", _lastVertical);
            }
            return;
        }

        MoveByWaypoint();
    }

    //Hàm di chuyển tới điểm waypoint
    void MoveByWaypoint()
    {
        //Sử dụng index để xác nhận target trong mảng waypoints
        Transform target = _waypoints[_currentWaypointIndex];
        Vector2 dir = (target.position - transform.position).normalized;

        if(_anim != null && dir.magnitude > 0f)
        {
            _lastHorizontal = dir.x;
            _lastVertical = dir.y;
        }

        //Di chuyển và tính khoảng cách của object và target
        transform.position = Vector2.MoveTowards(transform.position, target.position, _moveSpeed * Time.deltaTime);
        if(_anim != null)
        {
            _anim.SetFloat("Horizontal", dir.x);
            _anim.SetFloat("Vertical", dir.y);
            _anim.SetBool("isWalking", dir.magnitude > 0f);
        }

        var distanceToWaypoint = Vector2.Distance(transform.position, target.position);
        //Nếu object đến gần waypoint thì sẽ bắt đầu chờ đợi để di chuyển đến waypoint tiếp theo
        if(distanceToWaypoint < .1f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    //Coroutine để cho object ngồi chờ và chuyển index đến waypoint tiếp theo
    IEnumerator WaitAtWaypoint()
    {
        _isWaiting = true;
        if (_anim != null) _anim.SetBool("isWalking", false);

        yield return new WaitForSeconds(_waitTime);

        //Nếu tick loopintWaypoints thì sẽ tự động chạy tuần hoàn khắp các waypoint trong mảng (di chuyển tuần hoàn không ngừng)
        //Nếu không tick thì sẽ di chuyển đến waypoint cuối rồi đứng yên
        _currentWaypointIndex = _loopingWaypoints ? (_currentWaypointIndex + 1) % _waypoints.Length : Mathf.Min(_currentWaypointIndex + 1, _waypoints.Length - 1);

        _isWaiting = false;
    }
}

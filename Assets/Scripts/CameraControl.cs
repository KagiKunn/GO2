using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // 상수 : 이동 관련
    private const float DirectionForceReduceRate = 0.935f; // 감속비율
    private const float DirectionForceMin = 0.001f; // 설정치 이하일 경우 움직임을 멈춤

    // 변수 : 이동 관련
    private bool _userMoveInput; // 현재 조작을 하고있는지 확인을 위한 변수
    private Vector3 _startPosition; // 입력 시작 위치를 기억
    private Vector3 _directionForce; // 조작을 멈췄을때 서서히 감속하면서 이동 시키기 위한 변수

    // 컴포넌트
    [SerializeField] private Camera _camera;
    private float temp_value;
    [SerializeField] private float speed = 10.0f;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        // 카메라 포지션 이동
        ControlCameraPosition();

        // 조작을 멈췄을때 감속
        ReduceDirectionForce();

        // 카메라 위치 업데이트
        UpdateCameraPosition();
    }

    private void ControlCameraPosition()
    {
        var mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            CameraPositionMoveStart(mouseWorldPosition);
        }
        else if (Input.GetMouseButton(0))
        {
            CameraPositionMoveProgress(mouseWorldPosition);
        }
        else
        {
            CameraPositionMoveEnd();
        }
    }

    private void CameraPositionMoveStart(Vector3 startPosition)
    {
        _userMoveInput = true;
        _startPosition = startPosition;
        _directionForce = Vector2.zero;
    }

    private void CameraPositionMoveProgress(Vector3 targetPosition)
    {
        if (!_userMoveInput)
        {
            CameraPositionMoveStart(targetPosition);
            return;
        }

        _directionForce = _startPosition - targetPosition;
    }

    private void CameraPositionMoveEnd()
    {
        _userMoveInput = false;
    }

    private void ReduceDirectionForce()
    {
        // 조작 중일때는 아무것도 안함
        if (_userMoveInput)
        {
            return;
        }

        // 감속 수치 적용
        _directionForce *= DirectionForceReduceRate;

        // 작은 수치가 되면 강제로 멈춤
        if (_directionForce.magnitude < DirectionForceMin)
        {
            _directionForce = Vector3.zero;
        }
    }

    private void UpdateCameraPosition()
    {
        // 이동 수치가 없으면 아무것도 안함
        if (_directionForce == Vector3.zero)
        {
            return;
        }

        var currentPosition = transform.position;
        var targetPosition = currentPosition + _directionForce;
        transform.position = Vector3.Lerp(currentPosition, targetPosition, 0.5f);
    }

    private void CameraZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * speed;

        // scroll < 0 : scroll down하면 줌인
        if (_camera.orthographicSize <= 2.67f && scroll > 0)
        {
            temp_value = _camera.orthographicSize;
            _camera.orthographicSize = temp_value; // maximize zoom in

            // 최대로 Zoom in 했을 때 특정 값을 지정했을 때

            // 최대 줌 인 범위를 벗어날 때 값에 맞추려고 한번 줌 아웃 되는 현상을 방지
        }
        
        // scroll > 0 : scroll up하면 줌아웃
        else if (_camera.orthographicSize >= 7.03f && scroll < 0)
        {
            temp_value = _camera.orthographicSize;
            _camera.orthographicSize = temp_value; // maximize zoom out
        }
        else
            _camera.orthographicSize -= scroll * 0.5f;
    }
}
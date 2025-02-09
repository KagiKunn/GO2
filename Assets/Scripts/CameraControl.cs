using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

#pragma warning disable CS0108, CS0114

public class CameraControl : MonoBehaviour {
	private const float DirectionForceReduceRate = 0.935f; // 감속비율
	private const float DirectionForceMin = 0.001f; // 설정치 이하일 경우 움직임을 멈춤

	private bool _userMoveInput; // 현재 조작을 하고있는지 확인을 위한 변수
	private Vector3 _startPosition; // 입력 시작 위치를 기억
	private Vector3 _directionForce; // 조작을 멈췄을때 서서히 감속하면서 이동 시키기 위한 변수

	[SerializeField] private Camera camera;
	private int currentCameraIndex;
	[SerializeField] private Vector3[] initialCameraPositions;
	private float temp_value;
	[SerializeField] private float speed = 10.0f;
	[SerializeField] private Tilemap[] tilemaps; // 타일맵 배열
	[SerializeField] private bool allway; // xy이동 / false면 y축만이동
	[SerializeField] private UIDocument uiDocument; // 최상단에 위치한 UI 도큐먼트

	private Vector3 minBounds;
	private Vector3 maxBounds;
	private float halfHeight;
	private float halfWidth;

	private Button cameraButton;
	private bool fliped;

	private void Awake() {
		cameraButton = uiDocument.rootVisualElement.Q<Button>("CameraButton");
		cameraButton.clicked += SwitchTilemap;
	}

	private void Start() {
		InitializeCamera();
	}

	private void InitializeCamera() {
		// 카메라의 반높이와 반너비를 계산
		halfHeight = camera.orthographicSize;
		halfWidth = halfHeight * camera.aspect;

		// 현재 타일맵의 경계를 가져옴
		UpdateBounds(tilemaps[currentCameraIndex]);
	}

	private void UpdateBounds(Tilemap tilemap) {
		Transform gridTransform = tilemap.layoutGrid.transform;
		Bounds tilemapBounds = tilemap.localBounds;

		minBounds = gridTransform.TransformPoint(tilemapBounds.min);
		maxBounds = gridTransform.TransformPoint(tilemapBounds.max);
	}

	private void Update() {
		ControlCameraPosition();
		ReduceDirectionForce();
		UpdateCameraPosition();
		CameraZoom();
		HandleTouchInput();
		
		if (Input.GetKeyDown(KeyCode.Space)){
			SwitchTilemap();	
		}
	}

	private void ControlCameraPosition() {
		var mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);

		if (Input.GetMouseButtonDown(0)) {
			CameraPositionMoveStart(mouseWorldPosition);
		} else if (Input.GetMouseButton(0)) {
			CameraPositionMoveProgress(mouseWorldPosition);
		} else {
			CameraPositionMoveEnd();
		}
	}

	private void CameraPositionMoveStart(Vector3 startPosition) {
		_userMoveInput = true;
		_startPosition = startPosition;
		_directionForce = Vector2.zero;
	}

	private void CameraPositionMoveProgress(Vector3 targetPosition) {
		if (!_userMoveInput) {
			CameraPositionMoveStart(targetPosition);

			return;
		}

		Vector3 movement = _startPosition - targetPosition;

		if (!allway) {
			movement.x = 0; // y축으로만 이동하도록 제한
		}

		_directionForce = movement;
	}

	private void CameraPositionMoveEnd() {
		_userMoveInput = false;
	}

	private void ReduceDirectionForce() {
		if (_userMoveInput) {
			return;
		}

		_directionForce *= DirectionForceReduceRate;

		if (_directionForce.magnitude < DirectionForceMin) {
			_directionForce = Vector3.zero;
		}
	}

	private void UpdateCameraPosition() {
		if (_directionForce == Vector3.zero) {
			return;
		}

		var currentPosition = camera.transform.position;
		var targetPosition = currentPosition + _directionForce;
		float clampedX = Mathf.Clamp(targetPosition.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);

		if (minBounds.x > maxBounds.x) {
			clampedX = Mathf.Clamp(targetPosition.x, maxBounds.x + halfWidth, minBounds.x - halfWidth);
		}

		float clampedY = Mathf.Clamp(targetPosition.y, minBounds.y + halfHeight + 20, maxBounds.y - halfHeight);

		if (!allway) {
			clampedX = currentPosition.x; // y축으로만 이동하도록 제한
		}

		camera.transform.position = new Vector3(clampedX, clampedY, targetPosition.z);
	}

	private void CameraZoom() {
		float scroll = Input.GetAxis("Mouse ScrollWheel") * speed;

		if (camera.orthographicSize <= 26.7f && scroll > 0) {
			temp_value = camera.orthographicSize;
			camera.orthographicSize = temp_value;
		} else if (camera.orthographicSize >= 100f && scroll < 0) {
			temp_value = camera.orthographicSize;
			camera.orthographicSize = temp_value;
		} else {
			camera.orthographicSize -= scroll * 5f;
		}

		halfHeight = camera.orthographicSize;
		halfWidth = halfHeight * camera.aspect;

		// 카메라의 위치를 경계 내로 조정
		var currentPosition = camera.transform.position;
		float clampedX = Mathf.Clamp(currentPosition.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);

		if (minBounds.x > maxBounds.x) {
			clampedX = Mathf.Clamp(currentPosition.x, maxBounds.x + halfWidth, minBounds.x - halfWidth);
		}

		float clampedY = Mathf.Clamp(currentPosition.y, minBounds.y +  + halfHeight + 20, maxBounds.y - halfHeight);
		camera.transform.position = new Vector3(clampedX, clampedY, currentPosition.z);
	}
	
	private void HandleTouchInput() {
		if (Input.touchCount == 1) {  // 터치 하나로 이동
			Touch touch = Input.GetTouch(0);
			Vector3 touchWorldPosition = camera.ScreenToWorldPoint(touch.position);

			if (touch.phase == TouchPhase.Began) {
				CameraPositionMoveStart(touchWorldPosition);
			} else if (touch.phase == TouchPhase.Moved) {
				CameraPositionMoveProgress(touchWorldPosition);
			} else if (touch.phase == TouchPhase.Ended) {
				CameraPositionMoveEnd();
			}
		} else if (Input.touchCount == 2) {  // 터치 두 개로 줌
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

			float difference = currentMagnitude - prevMagnitude;
			float newOrthographicSize = camera.orthographicSize - difference * speed * Time.deltaTime;

			// 카메라 줌 범위를 26.7f와 100f 사이로 제한
			if (newOrthographicSize < 26.7f) {
				newOrthographicSize = 26.7f;
			} else if (newOrthographicSize > 100f) {
				newOrthographicSize = 100f;
			}

			camera.orthographicSize = newOrthographicSize;

			// 카메라의 위치를 경계 내로 조정
			halfHeight = camera.orthographicSize;
			halfWidth = halfHeight * camera.aspect;

			var currentPosition = camera.transform.position;
			float clampedX = Mathf.Clamp(currentPosition.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
			float clampedY = Mathf.Clamp(currentPosition.y, minBounds.y + halfHeight + 20, maxBounds.y - halfHeight);
			camera.transform.position = new Vector3(clampedX, clampedY, currentPosition.z);
		}
	}


	private void SwitchTilemap() {
		// 현재 카메라 위치를 저장
		initialCameraPositions[currentCameraIndex] = camera.transform.position;

		// 다음 카메라 인덱스로 증가
		currentCameraIndex = (currentCameraIndex + 1) % tilemaps.Length;

		// 새로운 타일맵의 경계를 가져옴
		UpdateBounds(tilemaps[currentCameraIndex]);

		camera.transform.position = initialCameraPositions[currentCameraIndex];
	}
}
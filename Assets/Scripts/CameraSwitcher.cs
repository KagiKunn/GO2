using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField]
    private Camera[] cameras; // 여러 카메라를 받기 위한 배열
    [SerializeField]
    private UIDocument uiDocument; // 최상단에 위치한 UI 도큐먼트

    private int currentCameraIndex = 1; // 현재 활성화된 카메라 인덱스 (0 Left, 1 Center, 2 Right)
    private Button cameraButton;
    private AudioListener[] audioListeners;
    private CameraControl[] cameraControls; // CameraControl 스크립트 배열
    private Vector3[] initialCameraPositions; // 각 카메라의 초기 위치 저장 배열

    private void Awake()
    {
        cameraButton = uiDocument.rootVisualElement.Q<Button>("CameraButton");
        cameraButton.clicked += nextCamera;

        // 각 카메라에 AudioListener와 CameraControl이 있는지 확인하고 배열에 저장
        audioListeners = new AudioListener[cameras.Length];
        cameraControls = new CameraControl[cameras.Length];
        initialCameraPositions = new Vector3[cameras.Length];
        for (int i = 0; i < cameras.Length; i++)
        {
            audioListeners[i] = cameras[i].GetComponent<AudioListener>();
            if (audioListeners[i] == null)
            {
                audioListeners[i] = cameras[i].gameObject.AddComponent<AudioListener>();
            }

            cameraControls[i] = cameras[i].GetComponent<CameraControl>();

            // 각 카메라의 초기 위치 저장
            initialCameraPositions[i] = cameras[i].transform.position;
        }
    }

    void Start()
    {
        // 모든 카메라와 AudioListener, CameraControl을 비활성화하고 현재 카메라와 AudioListener, CameraControl을 활성화합니다.
        for (int i = 0; i < cameras.Length; i++)
        {
            bool isActive = (i == currentCameraIndex);
            cameras[i].enabled = isActive;
            audioListeners[i].enabled = isActive;
            if (cameraControls[i] != null)
            {
                cameraControls[i].enabled = isActive;

                // CameraControl 초기화
                if (isActive)
                {
                    cameraControls[i].InitializeCamera();
                }
            }
        }
    }

    void nextCamera()
    {
        if (currentCameraIndex == cameras.Length - 1)
        {
            SwitchToCamera(0);
        }
        else
        {
            SwitchToCamera(currentCameraIndex + 1);
        }
    }

    public void SwitchToCamera(int cameraIndex)
    {
        if (cameraIndex < 0 || cameraIndex >= cameras.Length)
        {
            CustomLogger.LogError("Invalid camera index: " + cameraIndex);
            // Debug.LogError("Invalid camera index: " + cameraIndex);
            return;
        }

        // 현재 카메라 위치 저장
        Vector3 currentCameraPosition = cameras[currentCameraIndex].transform.position;

        // 현재 카메라와 AudioListener, CameraControl 비활성화
        cameras[currentCameraIndex].enabled = false;
        audioListeners[currentCameraIndex].enabled = false;
        if (cameraControls[currentCameraIndex] != null)
        {
            cameraControls[currentCameraIndex].enabled = false;
        }

        // 새로운 카메라와 AudioListener 활성화
        currentCameraIndex = cameraIndex;
        cameras[currentCameraIndex].enabled = true;
        audioListeners[currentCameraIndex].enabled = true;

        // 새로운 카메라를 초기 위치로 이동
        cameras[currentCameraIndex].transform.position = initialCameraPositions[currentCameraIndex];

        // CameraControl 초기화 및 활성화
        if (cameraControls[currentCameraIndex] != null)
        {
            cameraControls[currentCameraIndex].enabled = true;
            cameraControls[currentCameraIndex].InitializeCamera();
        }
        CustomLogger.Log(currentCameraIndex + " Camera is Enabled","yellow");
        CustomLogger.Log(cameras[currentCameraIndex].transform.position + ": Camera position","yellow");
        //Debug.LogWarning(currentCameraIndex + " Camera is Enabled");
    }
}

using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float dragSpeed = 10.0f; // 화면 움직임 속도
    private float firstClickPointX;
    private float firstClickPointY;

    private void Start()
    {
    }

    private void Update()
    {
        ViewChange();
    }

    private void ViewChange()
    {
        if (Input.GetMouseButtonDown(0))
        {
            firstClickPointX = Input.mousePosition.x;
            firstClickPointY = Input.mousePosition.y;
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 postion = mainCamera.ScreenToViewportPoint(-new Vector3(
                Input.mousePosition.x - firstClickPointX, Input.mousePosition.y - firstClickPointY, 0));
            var move = postion * (Time.deltaTime * dragSpeed);
            mainCamera.transform.Translate(move);
        }
    }
}
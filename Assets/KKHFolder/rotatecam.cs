using UnityEngine;

public class FlipCameraRotation : MonoBehaviour
{
    private bool isFlipped = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // F키를 눌러서 반전
        {
            FlipCamera();
        }
    }

    void FlipCamera()
    {
        // 카메라의 현재 회전 각도를 가져옵니다.
        Vector3 eulerRotation = transform.eulerAngles;

        // x축 방향으로 180도 회전시킵니다.
        eulerRotation.y += 180;

        // 회전 각도를 업데이트합니다.
        transform.eulerAngles = eulerRotation;

        // 반전 상태를 토글합니다.
        isFlipped = !isFlipped;
    }
}
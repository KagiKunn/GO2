using UnityEngine;

public class FlipCamera : MonoBehaviour
{
    private bool isFlipped = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // F키를 눌러서 반전
        {
            FlipCameraView();
        }
    }

    void FlipCameraView()
    {
        // 현재 카메라의 스케일을 가져옵니다.
        Vector3 scale = transform.localScale;

        // x축 방향으로 스케일을 반전시킵니다.
        scale.x *= -1;

        // 스케일을 업데이트합니다.
        transform.localScale = scale;

        // 반전 상태를 토글합니다.
        isFlipped = !isFlipped;
    }
}
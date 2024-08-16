using UnityEngine;

public class FollowMouse : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        Follow();
    }
    void Follow()
    {
        CustomLogger.Log("-FollowMouse-");
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10.0f; // 카메라에서 떨어진 거리
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if (gameObject != null)
        {
            gameObject.transform.position = worldPosition;
        }
    }
}

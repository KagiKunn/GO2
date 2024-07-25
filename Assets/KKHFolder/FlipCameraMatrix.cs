using UnityEngine;

public class FlipCameraMatrix : MonoBehaviour
{
    private Camera cameraComponent;

    void Start()
    {
        cameraComponent = GetComponent<Camera>();
    }

    void OnPreCull()
    {
        Matrix4x4 matrix = cameraComponent.projectionMatrix;
        matrix.m00 *= -1; // x축 반전
        cameraComponent.projectionMatrix = matrix;
    }
}
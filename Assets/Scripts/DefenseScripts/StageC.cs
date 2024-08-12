using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageC : MonoBehaviour
{
    [SerializeField] private Canvas gameOverCanvas; // 게임오버 UI 관련 참조
    [SerializeField] private Image gameOverImage;
    [SerializeField] private Button gameOverButton;

    private void Awake()
    {
        // 초기화
        InitializeGameOverUI();
    }

    private void InitializeGameOverUI()
    {
        if (gameOverCanvas != null)
        {
            gameOverCanvas.enabled = false;
        }

        if (gameOverImage != null)
        {
            gameOverImage.enabled = false;
        }

        if (gameOverButton != null)
        {
            gameOverButton.enabled = false;
            gameOverButton.onClick.AddListener(OnGameOverButtonClick);
        }
    }

    public void ShowGameOverUI()
    {
        if (gameOverCanvas != null)
        {
            gameOverCanvas.enabled = true;
        }

        if (gameOverImage != null)
        {
            gameOverImage.enabled = true;
        }

        if (gameOverButton != null)
        {
            gameOverButton.enabled = true;
        }

        Time.timeScale = 0f; // 게임 일시 정지
    }

    private void OnGameOverButtonClick()
    {
        Debug.Log("버튼 클릭됨");
        SceneManager.LoadScene("Title");
    }
}
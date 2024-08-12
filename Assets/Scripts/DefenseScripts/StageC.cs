using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageC : MonoBehaviour
{
    [SerializeField] private Canvas gameOverCanvas; // 게임오버 UI 관련 참조
    [SerializeField] private Image gameOverImage;
    [SerializeField] private Button gameOverButton;

    [SerializeField] private Canvas stageClearCanvas; // 스테이지 클리어 UI 관련 참조
    [SerializeField] private Image stageClearImage;
    [SerializeField] private Button stageClearButton;

    private void Awake()
    {
        // 초기화
        InitializeGameOverUI();
        InitializeStageClearUI();
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

    private void InitializeStageClearUI()
    {
        if (stageClearCanvas != null)
        {
            stageClearCanvas.enabled = false;
        }

        if (stageClearImage != null)
        {
            stageClearImage.enabled = false;
        }

        if (stageClearButton != null)
        {
            stageClearButton.enabled = false;
            stageClearButton.onClick.AddListener(OnStageClearButtonClick);
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

    public void ShowStageClearUI()
    {
        if (stageClearCanvas != null)
        {
            stageClearCanvas.enabled = true;
        }

        if (stageClearImage != null)
        {
            stageClearImage.enabled = true;
        }

        if (stageClearButton != null)
        {
            stageClearButton.enabled = true;
        }

        Time.timeScale = 0f; // 게임 일시 정지
    }

    private void OnGameOverButtonClick()
    {
        Debug.Log("게임 오버 버튼 클릭됨");
        SceneManager.LoadScene("Title");
    }

    private void OnStageClearButtonClick()
    {
        Debug.Log("스테이지 클리어 버튼 클릭됨");
        SceneManager.LoadScene("InternalAffairs");
    }
}

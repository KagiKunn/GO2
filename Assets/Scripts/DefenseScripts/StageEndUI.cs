using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageEndUI : MonoBehaviour
{
    public GameObject changeSceneButtonPrefab; // UI 버튼 프리팹
    private GameObject changeSceneButtonInstance;

    private void Start()
    {
        if (changeSceneButtonInstance == null && changeSceneButtonPrefab != null)
        {
            changeSceneButtonInstance = Instantiate(changeSceneButtonPrefab, FindObjectOfType<Canvas>().transform);
            changeSceneButtonInstance.SetActive(false); // 처음에 비활성화
            Button button = changeSceneButtonInstance.GetComponent<Button>();
            button.onClick.AddListener(ChangeSceneButton);
        }
    }

    // 버튼을 생성하고 설정하는 메서드
    public void ShowChangeSceneButton()
    {
        if (changeSceneButtonInstance != null)
        {
            changeSceneButtonInstance.SetActive(true);
        }
    }

    // 버튼 클릭 시 씬을 전환하는 메서드
    private void ChangeSceneButton()
    {
        // 게임 정지 해제
        Time.timeScale = 1;
        // HeroManagement 씬으로 전환
        SceneManager.LoadScene("HeroManagement");
    }
}
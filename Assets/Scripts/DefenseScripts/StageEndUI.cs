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
            // 버튼을 생성하고 설정
            changeSceneButtonInstance = Instantiate(changeSceneButtonPrefab, FindObjectOfType<Canvas>().transform);
            changeSceneButtonInstance.SetActive(false); // 처음에 비활성화

            Button button = changeSceneButtonInstance.GetComponent<Button>();
            button.onClick.AddListener(ChangeSceneButton);
        }

        // 보스 사망 이벤트 구독
        EnemyMovement.OnBossDie += OnBossDie;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        EnemyMovement.OnBossDie -= OnBossDie;
    }

    // 보스 사망 시 호출되는 메서드
    private void OnBossDie()
    {
        SceneManager.LoadScene("HeroManagement");
        ShowChangeSceneButton();
    }

    // 버튼을 생성하고 설정하는 메서드
    public void ShowChangeSceneButton()
    {
        Debug.Log("ShowChangeSceneButton 호출됨");
        if (changeSceneButtonInstance != null)
        {
            changeSceneButtonInstance.SetActive(true);
        }
    }

    // 버튼 클릭 시 씬을 전환하는 메서드
    public void ChangeSceneButton()
    {
        Debug.Log("ChangeSceneButton 호출됨");
        // 게임 정지 해제
        Time.timeScale = 1;
        // HeroManagement 씬으로 전환
        SceneManager.LoadScene("HeroManagement");
    }
}
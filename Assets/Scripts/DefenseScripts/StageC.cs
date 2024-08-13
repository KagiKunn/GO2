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

    [SerializeField] private DefenseGameData defenseGameData; // ScriptableObject 참조
    [SerializeField] private int currentStageCount; // 현재 stageCount 값을 인스펙터에서 확인

    private EnemySpawner enemySpawner;

    private void Awake()
    {
        // 초기화
        InitializeGameOverUI();
        InitializeStageClearUI();

        // EnemySpawner 찾기
        enemySpawner = FindObjectOfType<EnemySpawner>();

        // EnemySpawner가 존재하는지 확인
        if (enemySpawner != null)
        {
            // stageRace 배열에서 랜덤으로 종족 선택 및 제거
            string selectedRace = SelectRandomRace();
            enemySpawner.SetSelectedRace(selectedRace); // 선택된 종족 설정
        }
        else
        {
            Debug.LogError("EnemySpawner를 찾을 수 없습니다!");
        }

        // 현재 stageCount 값 업데이트
        UpdateStageCount();
    }

    private void UpdateStageCount()
    {
        if (defenseGameData != null)
        {
            currentStageCount = defenseGameData.StageCount; // DefenseGameData의 stageCount 값을 가져옴
        }
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

    private string SelectRandomRace()
    {
        // 랜덤으로 stageRace 배열에서 종족 선택
        int randomIndex = Random.Range(0, defenseGameData.StageRace.Length);
        string selectedRace = defenseGameData.StageRace[randomIndex];

        // 선택된 종족을 배열에서 제거한 후 DefenseGameData의 stageRace에 재할당
        defenseGameData.StageRace = RemoveRaceAt(defenseGameData.StageRace, randomIndex);
        
        return selectedRace;
    }

    private string[] RemoveRaceAt(string[] array, int index)
    {
        string[] newArray = new string[array.Length - 1];
        for (int i = 0, j = 0; i < array.Length; i++)
        {
            if (i != index)
            {
                newArray[j++] = array[i];
            }
        }
        return newArray;
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

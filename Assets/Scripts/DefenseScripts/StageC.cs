using System;
using System.IO;
using InternalAffairs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.

public class StageC : MonoBehaviour
{
    public static StageC Instance { get; private set; }
    [SerializeField] private Canvas gameOverCanvas; // 게임오버 UI 관련 참조
    [SerializeField] private Image gameOverImage;
    [SerializeField] private Button gameOverButton;
    [SerializeField] private Canvas stageClearCanvas; // 스테이지 클리어 UI 관련 참조
    [SerializeField] private Image stageClearImage;
    [SerializeField] private Button stageClearButton;
    [SerializeField] private Canvas stageAllClearCanvas; // 스테이지 5까지 전부 클리어 UI 관련 참조
    [SerializeField] private Image stageAllClearImage;
    [SerializeField] private Button stageAllClearButton;
    [SerializeField] public string selectedRace; // PlayerLocalManager에서 받아온 종족

    private string saveFilePath;
    private EnemySpawner enemySpawner;
    [SerializeField] private CastleWallManager castleWallManager;
    [SerializeField] private TextMeshProUGUI stageInfoText;
    public bool isGamePaused;
    [SerializeField] private GameObject uiGameObject;
    private int currentWaveInStageC;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        enemySpawner = FindObjectOfType<EnemySpawner>();

        Time.timeScale = 1f;

        // 초기화
        InitializeGameOverUI();
        InitializeStageClearUI();
        InitializeAllClearUI();

        // EnemyRaceSelector 인스턴스에서 선택된 종족 받아오기
        selectedRace = PlayerLocalManager.Instance.lSelectedRace;
        CustomLogger.Log("로컬매니저에서 받아온 selectedRace : " + selectedRace, "black");

        // EnemySpawner가 존재하는지 확인
        if (enemySpawner != null)
        {
            enemySpawner.SetSelectedRace(selectedRace); // 선택된 종족을 스포너에 설정
            UpdateStageInfoText();
        }
        else
        {
            CustomLogger.LogError("EnemySpawner를 찾을 수 없습니다!");
        }
    }

    private void Update()
    {
        int currentWave = enemySpawner.currentWave;

        if (currentWaveInStageC != currentWave)
        {
            currentWaveInStageC = currentWave;
            UpdateStageInfoText();
        }
    }

    // 스테이지 정보 표시
    private void UpdateStageInfoText()
    {
        stageInfoText.text =
            $"Week:{PlayerSyncManager.Instance.Repeat}\nStage: {PlayerLocalManager.Instance.lStage}\nEnemy: {selectedRace}\nWave:{currentWaveInStageC}";
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

    private void UploadCastleWallDataToDefenseGameData()
    {
        if (castleWallManager != null && PlayerLocalManager.Instance != null)
        {
            PlayerLocalManager.Instance.lCastleMaxHp = castleWallManager.maxHealth;
            PlayerLocalManager.Instance.lCastleHp = castleWallManager.health;
            PlayerLocalManager.Instance.lCastleExtraHp = castleWallManager.extraHealth;
            Debug.Log("현재 시점의 CastleWall 데이터가 DefenseGameData에 업로드되었습니다.");
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

    private void InitializeAllClearUI()
    {
        if (stageAllClearCanvas != null)
        {
            stageAllClearCanvas.enabled = false;
        }

        if (stageAllClearImage != null)
        {
            stageAllClearImage.enabled = false;
        }

        if (stageAllClearButton != null)
        {
            stageAllClearButton.enabled = false;
            stageAllClearButton.onClick.AddListener(OnStageAllClearButtonClick);
        }
    }

    public void ShowGameOverUI()
    {
        isGamePaused = true;
        // 게임 종료시 메뉴UI들 버튼도 비활성화
        Menu menuScript = uiGameObject.GetComponent<Menu>();
        if (menuScript != null)
        {
            menuScript.DisableButtonInteractions();
        }

        Time.timeScale = 0f; // 게임 일시 정지
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
    }

    public void ShowStageClearUI()
    {
        isGamePaused = true;
        // 게임 종료시 메뉴UI들 버튼도 비활성화
        Menu menuScript = uiGameObject.GetComponent<Menu>();
        if (menuScript != null)
        {
            menuScript.DisableButtonInteractions();
        }

        UpdateRaceArray();
        
        Time.timeScale = 0f; // 게임 일시 정지

        // 최종 스테이지가 아닐 경우 현재 stageCount와 stageRace 배열 업데이트
        // 최종 스테이지일 경우 버튼 호출
        if (PlayerLocalManager.Instance.lStage == 5)
        {
            allStageClear();
            SaveGameData();
        }
        else
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
        }

        castleWallManager.SaveWallHP(); // 성벽 체력 상태 저장
        SaveGameData(); // DTO 스크립트를 세이브파일로 저장명령
    }

    private void OnGameOverButtonClick()
    {
        CustomLogger.Log("게임 오버 버튼 클릭됨");
        SceneManager.LoadScene("Title");
    }

    private void OnStageClearButtonClick()
    {
        CustomLogger.Log("스테이지 클리어 버튼 클릭됨");
        SceneManager.LoadScene("InternalAffairs");
    }

    private void OnStageAllClearButtonClick()
    {
        CustomLogger.Log("스테이지 올클리어 버튼 클릭됨");
        SceneManager.LoadScene("EndingCredit");
    }

    private void SaveGameData()
    {
        PlayerLocalManager.Instance.lSelectedRace = null; //선택된 종족데이터를 비워줌
        PlayerSyncManager.Instance.Save();
        PlayerLocalManager.Instance.Save();
        CustomLogger.Log("게임 데이터가 저장되었습니다: " + saveFilePath);
    }

    private void allStageClear()
    {
        // 로그라이크 포인트 증가
        PlayerSyncManager.Instance.RoguePoint += 1;
        PlayerLocalManager.Instance.lPoint += 1;
        Debug.Log("증가 전 위크값:" + PlayerSyncManager.Instance.Repeat);
        PlayerSyncManager.Instance.Repeat++;
        PlayerLocalManager.Instance.GoNextWeek(); // 스테이지와 체력 정보 초기화

        PlayerSyncManager.Instance.Save();
        PlayerLocalManager.Instance.Save();
        Debug.Log("증가 후 위크값:" + PlayerSyncManager.Instance.Repeat);

        isGamePaused = true;
        // 게임 종료시 메뉴 UI들 버튼도 비활성화
        Menu menuScript = uiGameObject.GetComponent<Menu>();
        if (menuScript != null)
        {
            menuScript.DisableButtonInteractions();
        }

        CustomLogger.Log("축하합니다, 최종 스테이지 클리어");
        if (stageAllClearCanvas != null)
        {
            stageAllClearCanvas.enabled = true;
        }

        if (stageAllClearImage != null)
        {
            stageAllClearImage.enabled = true;
        }

        if (stageAllClearButton != null)
        {
            stageAllClearButton.enabled = true;
        }
    }

    //종족배열을 업데이트하는 메서드들
    public void UpdateRaceArray()
    {
        var selectedRace = PlayerLocalManager.Instance.lSelectedRace;
        var stageRaceArray = PlayerLocalManager.Instance.lStageRace;

        if (!string.IsNullOrEmpty(selectedRace) && stageRaceArray.Length > 0)
        {
            // 선택된 종족을 배열에서 제거한 후 PlayerLocalManager의 lStageRace에 재할당
            int selectedIndex = Array.IndexOf(stageRaceArray, selectedRace);
            if (selectedIndex >= 0)
            {
                PlayerLocalManager.Instance.lStageRace = RemoveRaceAt(stageRaceArray, selectedIndex);
                PlayerLocalManager.Instance.UpdateStageCount();
                PlayerLocalManager.Instance.Save();

                string updatedRacesContent = string.Join(", ", PlayerLocalManager.Instance.lStageRace);
                CustomLogger.Log("종족 선택 후 세이브데이터 내 적 배열 목록: " + updatedRacesContent, "black");
                CustomLogger.Log("업데이트 후 스테이지카운트 : " + PlayerLocalManager.Instance.L_Stage, "white");
            }
        }
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
}
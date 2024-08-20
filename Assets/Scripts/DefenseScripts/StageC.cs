using System.IO;
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
    [SerializeField] private string[] currentStageRace; // 현재 stageRace 배열을 인스펙터에서 확인
    [SerializeField] public string selectedRace;

    private string saveFilePath;
    private EnemySpawner enemySpawner;
    [SerializeField] private CastleWallManager castleWallManager;
    [SerializeField] private TextMeshProUGUI stageInfoText;
    public bool isGamePaused;
    [SerializeField] private GameObject uiGameObject;
    private int currentWaveInStageC;

    private int randomIndex;
    
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


        // EnemySpawner 찾기
        enemySpawner = FindObjectOfType<EnemySpawner>();

        // EnemySpawner가 존재하는지 확인
        if (enemySpawner != null)
        {
            // stageRace 배열에서 랜덤으로 종족 선택 및 제거
            selectedRace = SelectRandomRace();
            enemySpawner.SetSelectedRace(selectedRace); // 선택된 종족 설정
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

    //스테이지 정보 표시
    private void UpdateStageInfoText()
    {
        stageInfoText.text = $"Week:{PlayerSyncManager.Instance.Repeat}\nStage: {PlayerLocalManager.Instance.lStage}\nEnemy: {selectedRace}\nWave:{currentWaveInStageC}";
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
            PlayerLocalManager.Instance.lCastleExtraHp = castleWallManager.extraHealth1;
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

    private string SelectRandomRace()
    {
        // 랜덤으로 stageRace 배열에서 종족 선택
        randomIndex = Random.Range(0, PlayerLocalManager.Instance.lStageRace.Length);
        selectedRace = PlayerLocalManager.Instance.lStageRace[randomIndex];

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
        isGamePaused = true;
        //게임 종료시 메뉴UI들 버튼도 비활성화
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

        //게임오버시 세이브파일 json삭제 처리
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            CustomLogger.Log("게임 오버로 인해 세이브 파일이 삭제되었습니다: " + saveFilePath);
        }
        else
        {
            CustomLogger.LogWarning("삭제할 세이브 파일이 존재하지 않습니다.");
        }
    }

    public void ShowStageClearUI()
    {
        isGamePaused = true;
        //게임 종료시 메뉴UI들 버튼도 비활성화
        Menu menuScript = uiGameObject.GetComponent<Menu>();
        if (menuScript != null)
        {
            menuScript.DisableButtonInteractions();
        }

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

        // 선택된 종족을 배열에서 제거한 후 DefenseGameData의 stageRace에 재할당
        PlayerLocalManager.Instance.lStageRace = RemoveRaceAt(PlayerLocalManager.Instance.lStageRace, randomIndex);
        PlayerLocalManager.Instance.UpdateStageCount();
        
        castleWallManager.SaveWallHP(); // 성벽 체력상태 저장
        SaveGameData(); //DTO스크립트를 세이브파일로 저장명령
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
        SceneManager.LoadScene("Title");
    }

    private void SaveGameData()
    {
        PlayerSyncManager.Instance.Save();
        PlayerLocalManager.Instance.Save();
        CustomLogger.Log("게임 데이터가 저장되었습니다: " + saveFilePath);
    }

    private void allStageClear()
    {
        //로그라이크 포인트 증가
        PlayerSyncManager.Instance.RoguePoint += 1;
        PlayerLocalManager.Instance.lPoint += 1;
        PlayerSyncManager.Instance.Save();
        PlayerLocalManager.Instance.Save();
       
        isGamePaused = true;
        //게임 종료시 메뉴UI들 버튼도 비활성화
        Menu menuScript = uiGameObject.GetComponent<Menu>();
        if (menuScript != null)
        {
            menuScript.DisableButtonInteractions();
        }

        CustomLogger.Log("축하합니다, 최종스테이지 클리어");
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

        PlayerSyncManager.Instance.Repeat++;
        PlayerLocalManager.Instance.GoNextWeek(); //스테이지와 체력정보 초기화
    }
}
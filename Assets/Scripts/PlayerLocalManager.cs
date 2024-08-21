using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PlayerLocalManager : MonoBehaviour
{
    private SceneControl sceneControl;
    private static string persistentDataPath;
    private string filePath;

    private int L_money;
    private int L_point;
    private int L_startGold;
    private int L_moreEarnGold;
    private int L_moreCastleHealth;
    private int L_reduceCooldown;
    private HeroList[] L_HeroeList;
    public int L_Week;
    public int L_Stage;
    public string[] L_StageRace;
    private string selectedRace;
    private float L_CastleMaxHP;
    private float L_CastleHP;
    private float L_CastleExtraHP;

    
    public int lMoney
    {
        get => L_money;
        set => L_money = value;
    }

    public int lPoint
    {
        get => L_point;
        set => L_point = value;
    }

    public int lStartGold
    {
        get => L_startGold;
        set => L_startGold = value;
    }

    public int lMoreEarnGold
    {
        get => L_moreEarnGold;
        set => L_moreEarnGold = value;
    }

    public int lMoreCastleHealth
    {
        get => L_moreCastleHealth;
        set => L_moreCastleHealth = value;
    }

    public int lReduceCooldown
    {
        get => L_reduceCooldown;
        set => L_reduceCooldown = value;
    }

    public HeroList[] lHeroeList
    {
        get => L_HeroeList;
        set => L_HeroeList = value;
    }
    
    public int lWeek
    {
        get => L_Week;
        set => L_Week = value;
    }
    public int lStage
    {
        get => L_Stage;
        set => L_Stage = value;
    }

    public string[] lStageRace
    {
        get => L_StageRace;
        set => L_StageRace = value;
    }

    public string SelectedRace
    {
        get => selectedRace;
        set => selectedRace = value;
    }
    
    public float lCastleMaxHp
    {
        get => L_CastleMaxHP;
        set => L_CastleMaxHP = value;
    }

    public float lCastleHp
    {
        get => L_CastleHP;
        set => L_CastleHP = value;
    }

    public float lCastleExtraHp
    {
        get => L_CastleExtraHP;
        set => L_CastleExtraHP = value;
    }


    public SceneControl SceneControl
    {
        get => sceneControl;
        set => sceneControl = value;
    }

    public static PlayerLocalManager Instance { get; private set; }

    private void Awake()
    {
        sceneControl = gameObject.AddComponent<SceneControl>();
        persistentDataPath = Application.persistentDataPath;
        filePath = Path.Combine(persistentDataPath, "Save.dat");

        if (File.Exists(filePath))
        {
            LoadLocalData();
        }
        else
        {
            CreateNewPlayer();
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void LoadLocalData()
    {
        if (File.Exists(filePath))
        {
            using (FileStream file = File.Open(filePath, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                PlayerLocalData localData = (PlayerLocalData)formatter.Deserialize(file);

                // Deserialize된 데이터를 현재 클래스의 필드에 할당
                lMoney = localData.Money;
                lPoint = localData.RemainedPoint;
                lStartGold = localData.StartGold;
                lMoreEarnGold = localData.MoreEarnGold;
                lMoreCastleHealth = localData.MoreCastleHealth;
                lReduceCooldown = localData.ReduceCooldown;
                lHeroeList = localData.HerosList;
                lStage = localData.Stage;
                lStageRace = localData.StageRace;
                lCastleMaxHp = localData.CastleMaxHealth;
                lCastleHp = localData.CastleHealth;
                lCastleExtraHp = localData.CastleExtraHealth;
            }
        }
    }

    private void CreateNewPlayer()
    {
        PlayerLocalData localData = new PlayerLocalData(); // 기본 생성자 호출

        // 초기값 설정
        lMoney = localData.Money;
        lPoint = localData.RemainedPoint;
        lStartGold = localData.StartGold;
        lMoreEarnGold = localData.MoreEarnGold;
        lMoreCastleHealth = localData.MoreCastleHealth;
        lReduceCooldown = localData.ReduceCooldown;
        lHeroeList = localData.HerosList;
        lStage = localData.Stage;
        lStageRace = localData.StageRace;
        lCastleMaxHp = localData.CastleMaxHealth;
        lCastleHp = localData.CastleHealth;
        lCastleExtraHp = localData.CastleExtraHealth;

        SaveLocalData(localData);
    }

    
    public void ResetStageRace()
    {
        lStage = 0;
        lStageRace = new string[] { "Human", "DarkElf", "Orc", "Witch", "Skeleton" };
        UpdateStageCount(); // 배열 리셋 후 stageCount도 동기화
    }

    // stageRace 배열의 길이에 따라 stageCount를 동기화
    public void UpdateStageCount()
    {
        lStage = 5 - lStageRace.Length; // 남은 종족 수에 따라 stageCount를 계산
        Save();
    }

    // 성벽 체력 데이터를 기본값으로 재설정하는 메서드
    public void ResetHealthData()
    {
        lCastleMaxHp = 30000f; // 기본값으로 재설정
        lCastleHp = 30000; // 기본값으로 재설정
        lCastleExtraHp = 0f; // 기본값으로 재설정
        Debug.Log("성벽 체력 데이터가 기본값으로 재설정되었습니다.");
        Save();
    }
        
    public void GoNextWeek()
    {
        CustomLogger.Log("GoNextWeek()호출", "orange");
        ResetStageRace();
        ResetHealthData();
    }
    
    public void Save()
    {
        PlayerLocalData localData = new PlayerLocalData(lMoney, lPoint, lStartGold, lMoreEarnGold, lMoreCastleHealth,
            lReduceCooldown, lHeroeList, lStage, lStageRace, lCastleMaxHp, lCastleHp, lCastleExtraHp);
        SaveLocalData(localData);
    }

    private void SaveLocalData(PlayerLocalData data)
    {
        using (FileStream file = File.Create(filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(file, data);
        }
    }

    void OnApplicationQuit()
    {
        Destroy(this);
    }
}
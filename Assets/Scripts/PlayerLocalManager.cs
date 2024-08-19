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

        SaveLocalData(localData);
    }

    public void Save()
    {
        PlayerLocalData localData = new PlayerLocalData(lMoney, lPoint,lStartGold, lMoreEarnGold, lMoreCastleHealth, lReduceCooldown, lHeroeList);
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

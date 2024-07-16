using System;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerDataControl : MonoBehaviour
{
    private SceneControl sc;
    private static string persistentDataPath;
    private string filepath;
    private string uuid;
    private int lv;
    private int repeat;

    private async void Awake()
    {
        sc = gameObject.AddComponent<SceneControl>();
        persistentDataPath = Application.persistentDataPath;
        filepath = Path.Combine(persistentDataPath, "Player.dat");

        if (File.Exists(filepath))
        {
            await LoadPlayer();
        }

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        // 필요시 초기화 작업 추가
    }

    public void GameStart()
    {
        if (string.IsNullOrEmpty(uuid))
        {
            MakePlayer();
        }
        else
        {
            Login();
        }

        sc.LoadScene("Main");
    }

    private async Task LoadPlayer()
    {
        try
        {
            using (FileStream file = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                PlayerLocalData pld = (PlayerLocalData)formatter.Deserialize(file);

                uuid = pld.uuid;
                lv = pld.lv;
                repeat = pld.repeat;

                await Task.Delay(10); // 비동기 작업 시 모의 딜레이
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load save: {ex.Message}");
        }
    }

    private void MakePlayer()
    {
        uuid = Guid.NewGuid().ToString();

        PlayerLocalData pld = new PlayerLocalData
        {
            uuid = uuid,
            lv = 1,
            repeat = 0
        };

        SaveLocalData(pld);

        DBControl.OnCUD($"Insert into account values (null, '{uuid}', {pld.lv}, {pld.repeat})");
    }

    private void Login()
    {
        DataSet ds = DBControl.OnRead($"Select * from account where uuid = '{uuid}'", "account");

        if (ds != null && ds.Tables.Contains("account"))
        {
            DataTable dt = ds.Tables["account"];
            DataRow row = dt.Rows[0];

            uuid = row["uuid"].ToString();
            lv = Convert.ToInt32(row["lv"]);
            repeat = Convert.ToInt32(row["repeat"]);
        }

        Save();
    }

    private void Save()
    {
        PlayerLocalData pld = new PlayerLocalData
        {
            uuid = uuid,
            lv = lv,
            repeat = repeat
        };

        SaveLocalData(pld);
    }

    private void SaveLocalData(PlayerLocalData data)
    {
        try
        {
            using (FileStream file = File.Create(filepath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(file, data);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save data: {ex.Message}");
        }
    }
}

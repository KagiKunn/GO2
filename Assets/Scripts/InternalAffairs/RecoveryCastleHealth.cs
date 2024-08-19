using System;
using System.IO;
using DefenseScripts;
using UnityEngine;

public class RecoveryCastleHealth : MonoBehaviour
{
    private string path;
    private float maxHealth;
    private float crntHealth;
    private string json;
    private CastlGameData info;

    private void Awake()
    {
        string savePath = Path.Combine(Application.dataPath, "save", "DefenseData");
        path = Path.Combine(savePath, "DefenseGameData.json");
        Debug.Log("성벽 리커버리 json경로 : "+ path);
        if (File.Exists(path))
        {
            json = File.ReadAllText(path);
            info = JsonUtility.FromJson<CastlGameData>(json);
            maxHealth = info.MaxHealth;
            crntHealth = info.Health;
        }
        else
        {
            CustomLogger.Log("DefenseGameData.json does not exist");
        }
    }

    public void OnRecovery()
    {
        if (Mathf.Approximately(maxHealth, crntHealth))
        {
            CustomLogger.Log("HP is already full!");
            return;
        }
        
        // 여기에 골드 연동 관련 로직 추가
        // 예: playerGold -= recoveryCost;

        // 체력을 회복하고 데이터를 저장
        info.Health = info.MaxHealth;
        SaveHealthData();
    }
    
    float EmptyHealth()
    {
        return maxHealth - crntHealth;
    }

    private void SaveHealthData()
    {
        // maxHealth와 health를 업데이트한 데이터를 JSON 파일에 저장
        json = JsonUtility.ToJson(info, true); // true는 JSON 데이터를 보기 좋게 포맷팅
        File.WriteAllText(path, json);
        CustomLogger.Log("Health data saved to " + path);
    }
}

[Serializable]
public class CastlGameData
{
    public float MaxHealth;
    public float Health;
    public float ExtraHealth;
    public string[] StageRace;
    public int StageCount;
}
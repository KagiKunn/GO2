using System;
using System.IO;
using UnityEngine;

public class RecoveryCastleHealth : MonoBehaviour
{
    private string path;
    private float maxHealth;
    private float crntHealth;
    private string json;
    private DefenseGameData info;

    private void Awake()
    {
        path = Path.Combine(Application.dataPath, "save", "DefenseData", "DefenseGameData.json");
        if (File.Exists(path))
        {
            json = File.ReadAllText(path);
            info = JsonUtility.FromJson<DefenseGameData>(json);
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
        if (maxHealth == crntHealth)
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
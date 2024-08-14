using System.IO;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public PlayerDataScriptableObject playerDataScriptableObject;

    private string saveFilePath;

    private void Start()
    {
        // 저장 경로 설정
        saveFilePath = Path.Combine(Application.dataPath, "save","PlayerData.json");

        // JSON 파일에서 데이터 불러오기
        playerDataScriptableObject.LoadFromJson(saveFilePath);

        // 불러온 데이터 확인
        Debug.Log("Player ID: " + playerDataScriptableObject.data.playerId);
        Debug.Log("Money: " + playerDataScriptableObject.data.money);
        Debug.Log("Soul: " + playerDataScriptableObject.data.soul);
    }

    public void AddMoney(int money)
    {
        playerDataScriptableObject.data.money += money;
    }
    public void AddSoul(int soul)
    {
        playerDataScriptableObject.data.soul += soul;
    }

    public int GetMoney()
    {
        return playerDataScriptableObject.data.money;
    }
    public int GetSoul()
    {
        return playerDataScriptableObject.data.money;
    }

    public PlayerDataScriptableObject o
    {
        get => playerDataScriptableObject;
        set => playerDataScriptableObject = value;
    }
}
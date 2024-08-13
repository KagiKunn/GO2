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

    public void SaveData()
    {
        // JSON 파일로 데이터 저장
        playerDataScriptableObject.SaveToJson(saveFilePath);
        Debug.Log("Data saved to " + saveFilePath);
    }
}
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "DefenseGameData", menuName = "Game Data/Defense Game Data")]
public class DefenseGameData : ScriptableObject
{
    [SerializeField] private string[] stageRace = { "Human", "DarkElf", "Orc", "Witch", "Skeleton" };
    [SerializeField] private int stageCount;

    // stageRace 배열의 값을 리셋하고 stageCount와 동기화
    public void ResetStageRace()
    {
        stageRace = new string[] { "Human", "DarkElf", "Orc", "Witch", "Skeleton" };
        UpdateStageCount(); // 배열 리셋 후 stageCount도 동기화
    }

    // stageRace 배열의 길이에 따라 stageCount를 동기화
    public void UpdateStageCount()
    {
        stageCount = 5 - stageRace.Length; // 남은 종족 수에 따라 stageCount를 계산
    }

    // stageRace 배열을 외부에서 접근 + 수정할 수 있도록 하는 프로퍼티(getter + setter)
    public string[] StageRace
    {
        get => stageRace;
        set
        {
            stageRace = value;
            UpdateStageCount(); // 배열이 변경되면 stageCount를 업데이트
        }
    }
    // stageCount를 외부에서 접근할 수 있도록 하는 프로퍼티
    public int StageCount => stageCount;
    
    // JSON 형식으로 데이터를 저장하는 메서드
    public void SaveToJson(string filePath)
    {
        string json = JsonUtility.ToJson(this, true); // 객체를 JSON으로 직렬화
        File.WriteAllText(filePath, json); // JSON을 파일에 저장
    }

    // JSON 형식으로 데이터를 불러오는 메서드
    public void LoadFromJson(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath); // 파일에서 JSON 읽기
            JsonUtility.FromJsonOverwrite(json, this); // JSON 데이터를 객체에 덮어쓰기
            CustomLogger.Log("세이브데이터 로드 완료. 경로 : " + filePath, "pink");
            UpdateStageCount(); // 불러온 후 stageCount를 업데이트
        }
        else
        {
            Debug.LogWarning("세이브 파일을 찾을 수 없습니다. 기본값으로 초기화합니다.");
            ResetStageRace(); // 기본값으로 초기화
            SaveToJson(filePath); //기본값 데이터를 세이브파일로 저장
        }
    }
    
}
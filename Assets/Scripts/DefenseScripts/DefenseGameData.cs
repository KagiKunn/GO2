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
}
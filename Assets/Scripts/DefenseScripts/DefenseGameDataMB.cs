using System;
using System.IO;
using UnityEngine;

namespace DefenseScripts
{
    public class DefenseGameDataMB : MonoBehaviour
    {
        //몇 회차인지 
        [SerializeField] public int weekCount = 1;
        [SerializeField] public int roguePoint = 0;
        
        [SerializeField] private string[] stageRace = { "Human", "DarkElf", "Orc", "Witch", "Skeleton" };
        [SerializeField] private int stageCount;

        // 성벽 체력 데이터
        [SerializeField] private float maxHealth = 30000f; // 성벽 체력의 기본값
        [SerializeField] private float health; // 기본값으로 100 설정
        [SerializeField] private float extraHealth = 0f; // 기본값으로 0 설정 

        private string saveFilePath;

        private void Awake()
        {
            CustomLogger.Log("DefenseGameDataMB 기동", "yellow");
            CustomLogger.Log("DGDMB Awake()현재 stageCount : " + stageCount, "orange");
            if (stageCount == 0)
            {
                ResetStageRace();
                ResetHealthData();
                CustomLogger.Log("DGDMB Awake() 데이터 기본값 초기화 완료", "orange");
            }

            // Save file path 설정
            string savePath = Path.Combine(Application.dataPath, "save", "DefenseData");
            Directory.CreateDirectory(savePath); // 디렉터리가 없으면 생성

            // 파일 경로 설정
            saveFilePath = Path.Combine(savePath, "DefenseGameData.json");

            // 경로를 출력하여 확인
            CustomLogger.Log("MB Save file path: " + saveFilePath);

            //기동 시에 세이브파일 데이터 불러오기
            LoadFromJson(saveFilePath);
        }

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

        // weekCount를 외부에서 접근 + 수정할 수 있도록 하는 프로퍼티(getter + setter)
        public int WeekCount
        {
            get => weekCount;
            set => weekCount = value;
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

        // 성벽 데이터 변수 관리
        public float MaxHealth
        {
            get => maxHealth;
            set => maxHealth = value;
        }

        public float Health
        {
            get => health;
            set => health = value;
        }

        public float ExtraHealth
        {
            get => extraHealth;
            set => extraHealth = value;
        }

        // 성벽 체력 데이터를 기본값으로 재설정하는 메서드
        public void ResetHealthData()
        {
            maxHealth = 30000f; // 기본값으로 재설정
            health = maxHealth; // 기본값으로 재설정
            extraHealth = 0f; // 기본값으로 재설정
            Debug.Log("성벽 체력 데이터가 기본값으로 재설정되었습니다.");
        }

        // JSON 형식으로 데이터를 저장하는 메서드
        public void SaveToJson(string filePath)
        {
            CustomLogger.Log("DGD MB savetojson 호출완료");
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
                Debug.Log("세이브데이터 로드 완료. 경로 : " + filePath);
                UpdateStageCount(); // 불러온 후 stageCount를 업데이트
            }
            else
            {
                Debug.LogWarning("세이브 파일을 찾을 수 없습니다. 기본값으로 초기화합니다.");
                ResetStageRace(); // 기본값으로 초기화
                ResetHealthData(); // 체력 데이터도 기본값으로 초기화
                SaveToJson(filePath); // 기본값 데이터를 세이브파일로 저장
            }
        }

        public void GoNextWeek()
        {
            CustomLogger.Log("GoNextWeek()호출", "orange");
            ResetStageRace();
            ResetHealthData();
        }
    }
}
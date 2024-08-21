using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InternalAffairs
{
    
    public class EnemyRaceSelector : MonoBehaviour
    {
        public static EnemyRaceSelector Instance { get; private set; }
        [SerializeField] public string[] enemyRaces;
        [SerializeField] public string selectedRace;
        private EnemySpawner enemySpawner;
        private int randomIndex;
        public int stageCount;
        public int weekCount;
        
        
        private void Start()
        {
            CustomLogger.Log("EnemyRaceSelector Start()진입", "black");
            if (Instance == null)
            {
                Instance = this;

                DontDestroyOnLoad(this.gameObject);
            }
            else if (Instance != this)
            {
                Destroy(this.gameObject);
            }

            weekCount = PlayerSyncManager.Instance.Repeat;
            stageCount = PlayerLocalManager.Instance.L_Stage;
            CustomLogger.Log("위크카운트  : "+weekCount+", 스테이지카운트 : "+stageCount,"white");
            
            
            var enemyRaces = PlayerLocalManager.Instance.lStageRace;
            string enemyRacesContent = string.Join(", ", enemyRaces);
            CustomLogger.Log("세이브데이터 내 적 배열 목록: " + enemyRacesContent, "black");
            
            SelectRandomRace();
            CustomLogger.Log("selectedRace:" + selectedRace, "black");
            
            // 선택된 종족을 PlayerLocalManager에 저장
            PlayerLocalManager.Instance.SelectedRace = selectedRace;

            // 선택된 종족을 배열에서 제거한 후 PlayerLocalManager의 lStageRace에 재할당
            PlayerLocalManager.Instance.lStageRace = RemoveRaceAt(PlayerLocalManager.Instance.lStageRace, randomIndex);
            stageCount = PlayerLocalManager.Instance.L_Stage;
            PlayerLocalManager.Instance.UpdateStageCount();
        }

        private void SelectRandomRace()
        {
            // 랜덤으로 lStageRace 배열에서 종족 선택
            randomIndex = Random.Range(0, PlayerLocalManager.Instance.lStageRace.Length);
            selectedRace = PlayerLocalManager.Instance.lStageRace[randomIndex];
        }

        private string[] RemoveRaceAt(string[] array, int index)
        {
            string[] newArray = new string[array.Length - 1];

            for (int i = 0, j = 0; i < array.Length; i++)
            {
                if (i != index)
                {
                    newArray[j++] = array[i];
                }
            }

            return newArray;
        }
    }
}
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InternalAffairs
{
    public class EnemyRaceSelector999 : MonoBehaviour
    {
        public static EnemyRaceSelector999 Instance { get; private set; }
        [SerializeField] public string[] enemyRaces;
        [SerializeField] public string selectedRace;
        private EnemySpawner enemySpawner;
        private int randomIndex;
        public int stageCount;
        public int weekCount;


        private void Awake()
        {
            CustomLogger.Log("EnemyRaceSelector Awake()진입", "black");
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
            CustomLogger.Log("위크카운트  : " + weekCount + ", 업데이트 전 스테이지카운트 : " + stageCount, "white");

            CustomLogger.Log("save상 선택된 종족: " + PlayerLocalManager.Instance.lSelectedRace, "black");

            if (PlayerLocalManager.Instance.lSelectedRace == null) //종족이 선택되지 않은 경우에만 실행. 게임껐다켜도 다시 뽑지않도록
            {
                CustomLogger.Log("종족 선택되지 않음. 종족 선택으로 이행", "black");
                if (stageCount == 0)
                {
                    PlayerLocalManager.Instance.ResetHealthData();
                }

                var enemyRaces = PlayerLocalManager.Instance.lStageRace;
                string enemyRacesContent = string.Join(", ", enemyRaces);
                CustomLogger.Log("세이브데이터 내 적 배열 목록: " + enemyRacesContent, "black");

                SelectRandomRace();
                CustomLogger.Log("selectedRace:" + selectedRace, "black");

                // 선택된 종족을 PlayerLocalManager에 저장
                PlayerLocalManager.Instance.lSelectedRace = selectedRace;
                CustomLogger.Log("로컬매니저에 저장된 종족123" + PlayerLocalManager.Instance.lSelectedRace, "white");
                
                // 선택된 종족을 배열에서 제거한 후 PlayerLocalManager의 lStageRace에 재할당
                PlayerLocalManager.Instance.lStageRace = RemoveRaceAt(PlayerLocalManager.Instance.lStageRace, randomIndex);
                stageCount = PlayerLocalManager.Instance.L_Stage;
                PlayerLocalManager.Instance.UpdateStageCount();
                PlayerLocalManager.Instance.Save();
                CustomLogger.Log("업데이트 후 스테이지카운트 : " + stageCount, "white");
            }
            else
            {
                selectedRace = PlayerLocalManager.Instance.lSelectedRace;
                PlayerLocalManager.Instance.Save();
                CustomLogger.Log("Save데이터 상에 종족이 선택되어 있으므로 그 값을 받아옴 : " + selectedRace, "black");
            }
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
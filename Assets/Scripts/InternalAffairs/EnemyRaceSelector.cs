using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InternalAffairs
{
    
    public class EnemyRaceSelector : MonoBehaviour
    {
        public static EnemyRaceSelector Instance { get; private set; }
        
        [SerializeField] public string selectedRace;
        private EnemySpawner enemySpawner;
        private int randomIndex;
        
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                DontDestroyOnLoad(this.gameObject);
            }
            else if (Instance != this)
            {
                Destroy(this.gameObject);
            }
            CustomLogger.Log("EnemyRaceSelector Awake()진입", "black");
            SelectRandomRace();
            
            // 선택된 종족을 배열에서 제거한 후 PlayerLocalManager의 lStageRace에 재할당
            PlayerLocalManager.Instance.lStageRace = RemoveRaceAt(PlayerLocalManager.Instance.lStageRace, randomIndex);
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
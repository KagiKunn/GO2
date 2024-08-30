using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace InternalAffairs
{
    public class EnemyRaceSelector : MonoBehaviour
    {
        public static EnemyRaceSelector Instance { get; private set; }
        [SerializeField] public string[] enemyRaces;
        [SerializeField] public string selectedRace;
        private int randomIndex;
        public int stageCount;
        public int weekCount;
        public Sprite humanLogo;
        public Sprite darkElfLogo;
        public Sprite orcLogo;
        public Sprite skeletonLogo;
        public Sprite witchLogo;


        private void Awake()
        {
            CustomLogger.Log("EnemyRaceSelector Awake()진입", Color.cyan);
            PlayerLocalManager.Instance.UpdateStageCount();

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
            CustomLogger.Log("위크카운트  : " + weekCount + ", 스테이지카운트 : " + stageCount, "white");

            CustomLogger.Log("save상 선택된 종족: " + PlayerLocalManager.Instance.lSelectedRace, Color.cyan);

            if (string.IsNullOrEmpty(PlayerLocalManager.Instance.lSelectedRace))
            {
                CustomLogger.Log("종족 선택되지 않음. 종족 선택으로 이행", Color.cyan);

                if (stageCount == 1)
                {
                    //1스테이지일때 벽 체력정보 리셋

                    PlayerLocalManager.Instance.lCastleHp=PlayerLocalManager.Instance.lCastleMaxHp;
                }

                // 종족 선택 및 저장
                SelectAndSaveRace();
            }
            else
            {
                selectedRace = PlayerLocalManager.Instance.lSelectedRace;
                PlayerLocalManager.Instance.Save();
                CustomLogger.Log("Save데이터 상에 종족이 선택되어 있으므로 그 값을 받아옴 : " + selectedRace, Color.cyan);
                CustomLogger.Log(PlayerLocalManager.Instance.lNextEnemy, "yellow");

                if (PlayerLocalManager.Instance.lNextEnemy)
                {
                    GameObject.Find("NextEnemy").GetComponent<TMP_InputField>().text = selectedRace;
                    ChangeRaceImage(selectedRace);
                }
            }
        }

        private void ChangeRaceImage(string selectedRace1)
        {
            // RaceImage라는 이름의 게임오브젝트 찾기
            GameObject raceImageObject = GameObject.Find("RaceImage");

            if (raceImageObject != null)
            {
                // Image 컴포넌트 가져오기
                Image raceImageComponent = raceImageObject.GetComponent<Image>();

                if (raceImageComponent != null)
                {
                    // SelectedRace 값에 따라 이미지 스프라이트 변경
                    switch (selectedRace1)
                    {
                        case "Human":
                            raceImageComponent.sprite = humanLogo;
                            break;
                        case "DarkElf":
                            raceImageComponent.sprite = darkElfLogo;
                            break;
                        case "Orc":
                            raceImageComponent.sprite = orcLogo;
                            break;
                        case "Skeleton":
                            raceImageComponent.sprite = skeletonLogo;
                            break;
                        case "Witch":
                            raceImageComponent.sprite = witchLogo;
                            break;
                        default:
                            CustomLogger.Log("Unknown race selected.", Color.red);
                            break;
                    }
                }
            }
            else
            {
                CustomLogger.Log("RaceImage 오브젝트를 찾을 수 없습니다.", Color.red);
            }
        }

        
        private void SelectAndSaveRace()
        {
            SelectRandomRace();
            CustomLogger.Log("SelectedRace:" + selectedRace, Color.cyan);

            // 선택된 종족을 PlayerLocalManager에 저장
            PlayerLocalManager.Instance.lSelectedRace = selectedRace;
            CustomLogger.Log("로컬매니저에 저장된 종족 : " + PlayerLocalManager.Instance.lSelectedRace, "white");

            PlayerLocalManager.Instance.Save();
        }

        private void SelectRandomRace()
        {
            // 랜덤으로 lStageRace 배열에서 종족 선택
            randomIndex = Random.Range(0, PlayerLocalManager.Instance.lStageRace.Length);
            selectedRace = PlayerLocalManager.Instance.lStageRace[randomIndex];
        }
    }
}
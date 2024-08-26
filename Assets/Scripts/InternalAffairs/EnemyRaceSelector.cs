using System;
using TMPro;
using UnityEngine;

using Random = UnityEngine.Random;

namespace InternalAffairs {
	public class EnemyRaceSelector : MonoBehaviour {
		public static EnemyRaceSelector Instance { get; private set; }
		[SerializeField] public string[] enemyRaces;
		[SerializeField] public string SelectedRace;
		private int randomIndex;
		public int stageCount;
		public int weekCount;

		private void Awake() {
			CustomLogger.Log("EnemyRaceSelector Awake()진입", Color.cyan);
			PlayerLocalManager.Instance.UpdateStageCount();

			if (Instance == null) {
				Instance = this;
				DontDestroyOnLoad(this.gameObject);
			} else if (Instance != this) {
				Destroy(this.gameObject);
			}

			weekCount = PlayerSyncManager.Instance.Repeat;
			stageCount = PlayerLocalManager.Instance.L_Stage;
			CustomLogger.Log("위크카운트  : " + weekCount + ", 스테이지카운트 : " + stageCount, "white");

			CustomLogger.Log("save상 선택된 종족: " + PlayerLocalManager.Instance.lSelectedRace, "black");
			if (string.IsNullOrEmpty(PlayerLocalManager.Instance.lSelectedRace)) {
				CustomLogger.Log("종족 선택되지 않음. 종족 선택으로 이행", Color.cyan);

				if (stageCount == 1) { //1스테이지일때 벽 체력정보 리셋
					
					PlayerLocalManager.Instance.ResetHealthData();
				}

				// 종족 선택 및 저장
				SelectAndSaveRace();
			} else {
				SelectedRace = PlayerLocalManager.Instance.lSelectedRace;
				PlayerLocalManager.Instance.Save();
				CustomLogger.Log("Save데이터 상에 종족이 선택되어 있으므로 그 값을 받아옴 : " + SelectedRace, "black");
			}

			;
			if (GameObject.Find("DTO").GetComponent<NextEnemy>().isVisual)
			{
				GameObject.Find("NextEnemy").GetComponent<TextMeshProUGUI>().text = SelectedRace;
			}
		}

		private void SelectAndSaveRace() {
			SelectRandomRace();
			CustomLogger.Log("SelectedRace:" + SelectedRace, Color.cyan);

			// 선택된 종족을 PlayerLocalManager에 저장
			PlayerLocalManager.Instance.lSelectedRace = SelectedRace;
			CustomLogger.Log("로컬매니저에 저장된 종족 : " + PlayerLocalManager.Instance.lSelectedRace, "white");

			PlayerLocalManager.Instance.Save();
		}

		private void SelectRandomRace() {
			// 랜덤으로 lStageRace 배열에서 종족 선택
			randomIndex = Random.Range(0, PlayerLocalManager.Instance.lStageRace.Length);
			SelectedRace = PlayerLocalManager.Instance.lStageRace[randomIndex];
		}
	}
}
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.

public class StageC : MonoBehaviour {
	[SerializeField] private Canvas gameOverCanvas; // 게임오버 UI 관련 참조
	[SerializeField] private Image gameOverImage;
	[SerializeField] private Button gameOverButton;

	[SerializeField] private Canvas stageClearCanvas; // 스테이지 클리어 UI 관련 참조
	[SerializeField] private Image stageClearImage;
	[SerializeField] private Button stageClearButton;

	[SerializeField] private DefenseGameData defenseGameData; // ScriptableObject 참조
	[SerializeField] private int currentStageCount; // 현재 stageCount 값을 인스펙터에서 확인
	[SerializeField] private string[] currentStageRace; // 현재 stageRace 배열을 인스펙터에서 확인
	[SerializeField] private string selectedRace;

	private string saveFilePath;
	private EnemySpawner enemySpawner;

	private void Awake() {
		// Save file path 설정
		string savePath = Path.Combine(Application.dataPath, "save", "DefenseData");
		Directory.CreateDirectory(savePath); // 디렉터리가 없으면 생성

		// 파일 경로 설정
		saveFilePath = Path.Combine(savePath, "DefenseGameData.json");

		// 경로를 출력하여 확인
		CustomLogger.Log("Save file path: " + saveFilePath);
CustomLogger
		// JSON에서 데이터를 불러옴
		defenseGameData.LoadFromJson(saveFilePath);

		// 초기화
		InitializeGameOverUI();
		InitializeStageClearUI();

		// EnemySpawner 찾기
		enemySpawner = FindObjectOfType<EnemySpawner>();

		// EnemySpawner가 존재하는지 확인
		if (enemySpawner != null) {
			// stageRace 배열에서 랜덤으로 종족 선택 및 제거
			selectedRace = SelectRandomRace();
			enemySpawner.SetSelectedRace(selectedRace); // 선택된 종족 설정
		} else {
			CustomLogger.LogError("EnemySpawner를 찾을 수 없습니다!");
		}

		// 현재 stageCount와 stageRace 배열 업데이트
		UpdateStageData();
	}

	private void UpdateStageData() {
		if (defenseGameData != null) {
			currentStageCount = defenseGameData.StageCount; // DefenseGameData의 stageCount 값을 가져옴
			currentStageRace = defenseGameData.StageRace; // DefenseGameData의 stageRace 배열 값을 가져옴
		}
	}

	private void InitializeGameOverUI() {
		if (gameOverCanvas != null) {
			gameOverCanvas.enabled = false;
		}

		if (gameOverImage != null) {
			gameOverImage.enabled = false;
		}

		if (gameOverButton != null) {
			gameOverButton.enabled = false;
			gameOverButton.onClick.AddListener(OnGameOverButtonClick);
		}
	}

	private void InitializeStageClearUI() {
		if (stageClearCanvas != null) {
			stageClearCanvas.enabled = false;
		}

		if (stageClearImage != null) {
			stageClearImage.enabled = false;
		}

		if (stageClearButton != null) {
			stageClearButton.enabled = false;
			stageClearButton.onClick.AddListener(OnStageClearButtonClick);
		}
	}

	private string SelectRandomRace() {
		// 랜덤으로 stageRace 배열에서 종족 선택
		int randomIndex = Random.Range(0, defenseGameData.StageRace.Length);
		string selectedRace = defenseGameData.StageRace[randomIndex];

		// 선택된 종족을 배열에서 제거한 후 DefenseGameData의 stageRace에 재할당
		defenseGameData.StageRace = RemoveRaceAt(defenseGameData.StageRace, randomIndex);

		// stageRace 배열을 업데이트
		UpdateStageData();

		return selectedRace;
	}

	private string[] RemoveRaceAt(string[] array, int index) {
		string[] newArray = new string[array.Length - 1];

		for (int i = 0, j = 0; i < array.Length; i++) {
			if (i != index) {
				newArray[j++] = array[i];
			}
		}

		return newArray;
	}

	public void ShowGameOverUI() {
		if (gameOverCanvas != null) {
			gameOverCanvas.enabled = true;
		}

		if (gameOverImage != null) {
			gameOverImage.enabled = true;
		}

		if (gameOverButton != null) {
			gameOverButton.enabled = true;
		}

		Time.timeScale = 0f; // 게임 일시 정지
	}

	public void ShowStageClearUI() {
		if (stageClearCanvas != null) {
			stageClearCanvas.enabled = true;
		}

		if (stageClearImage != null) {
			stageClearImage.enabled = true;
		}

		if (stageClearButton != null) {
			stageClearButton.enabled = true;
		}

		Time.timeScale = 0f; // 게임 일시 정지
	}

	private void OnGameOverButtonClick() {
		CustomLogger.Log("게임 오버 버튼 클릭됨");

		//게임오버시 세이브파일 json삭제 처리
		if (File.Exists(saveFilePath)) {
			File.Delete(saveFilePath);
			CustomLogger.Log("게임 오버로 인해 세이브 파일이 삭제되었습니다: " + saveFilePath);
		} else {
			CustomLogger.LogWarning("삭제할 세이브 파일이 존재하지 않습니다.");
		}

		SceneManager.LoadScene("Title");
	}

	private void OnStageClearButtonClick() {
		CustomLogger.Log("스테이지 클리어 버튼 클릭됨");
		SaveGameData(); //버튼 클릭 시 세이브데이터 저장
		SceneManager.LoadScene("InternalAffairs");
	}

	private void SaveGameData() {
		defenseGameData.SaveToJson(saveFilePath);
		CustomLogger.Log("게임 데이터가 저장되었습니다: " + saveFilePath);
	}
}
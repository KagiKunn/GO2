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

	private string[] stageRace = { "Human", "DarkElf", "Orc", "Witch", "Skeleton" };
	private EnemySpawner enemySpawner;
	private int stageCount;

	private void Awake() {
		// 초기화
		InitializeGameOverUI();
		InitializeStageClearUI();

		// EnemySpawner 찾기
		enemySpawner = FindObjectOfType<EnemySpawner>();

		// EnemySpawner가 존재하는지 확인
		if (enemySpawner != null) {
			// stageRace 배열에서 랜덤으로 종족 선택 및 제거
			string selectedRace = SelectRandomRace();
			enemySpawner.SetSelectedRace(selectedRace); // 선택된 종족 설정
		} else {
			Debug.LogError("EnemySpawner를 찾을 수 없습니다!");
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
		// stageCount 계산
		stageCount = 6 - stageRace.Length;

		int randomIndex = Random.Range(0, stageRace.Length);
		string selectedRace = stageRace[randomIndex];

		// 선택된 종족을 배열에서 제거
		stageRace = RemoveRaceAt(stageRace, randomIndex);

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
		Debug.Log("게임 오버 버튼 클릭됨");
		SceneManager.LoadScene("Title");
	}

	private void OnStageClearButtonClick() {
		Debug.Log("스테이지 클리어 버튼 클릭됨");
		SceneManager.LoadScene("InternalAffairs");
	}
}
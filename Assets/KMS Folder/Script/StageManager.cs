using System;

using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class StageManager : MonoBehaviour {
	public GameObject[] leftStageUnits;
	public GameObject[] rightStageUnits;

	public Button leftButton;
	public Button rightButton;
	
	public PlacementUnitA leftWallStage;
	public PlacementUnitA rightWallStage;

	private UnitGameManagerA unitGameManager;
	private UnitSlotManagerA unitSlotManager;
	public PlacementUnitA placementUnit;

	private IEnumerator Start() {
		yield return new WaitUntil(() => UnitGameManagerA.Instance != null);

		unitGameManager = UnitGameManagerA.Instance;
		unitSlotManager = FindFirstObjectByType<UnitSlotManagerA>();
		
		// 초기 상태에 따라 placementUnit 설정
		if (leftButton.gameObject.activeSelf) {
			placementUnit = rightWallStage;
		} else if (rightButton.gameObject.activeSelf) {
			placementUnit = leftWallStage;
		}

		leftButton.onClick.AddListener(OnLeftButtonClicked);
		rightButton.onClick.AddListener(OnRightButtonClicked);
	}

	private void OnLeftButtonClicked() {
		// 1. 오른쪽 성벽(Right Wall Stage)을 PlacementUnit으로 설정
		placementUnit = rightWallStage;

		// 2. 현재 성벽 정보를 저장
		SaveCurrentWallPlacement();

		// 3. 성벽 전환
		SwitchToStage(leftStageUnits, rightStageUnits, leftButton, rightButton);
	}

	private void OnRightButtonClicked() {
		// 1. 왼쪽 성벽(Left Wall Stage)을 PlacementUnit으로 설정
		placementUnit = leftWallStage;

		// 2. 현재 성벽 정보를 저장
		SaveCurrentWallPlacement();

		// 3. 성벽 전환
		SwitchToStage(rightStageUnits, leftStageUnits, rightButton, leftButton);
	}

	private void SwitchToStage(GameObject[] stageToActivate, GameObject[] stageToDeactivate, Button buttonToDisable, Button buttonToEnable) {
		SetStageActive(stageToActivate, true);
		SetStageActive(stageToDeactivate, false);

		buttonToDisable.gameObject.SetActive(false);
		buttonToEnable.gameObject.SetActive(true);
	}

	private void SetStageActive(GameObject[] stageUnits, bool isActive) {
		foreach (var unit in stageUnits) {
			unit.SetActive(isActive);
		}
	}

	private void SaveCurrentWallPlacement() {
		if (placementUnit != null) {
			CustomLogger.Log(placementUnit.name);
			placementUnit.SavePlacementUnits();
		} else {
			CustomLogger.Log("활성화된 PlacementUnitA를 찾을 수 없습니다.", Color.red);
		}
	}

	public int GetCurrentWallStatus() {
		if (!leftButton.gameObject.activeSelf) {
			return 1;  // 왼쪽 성벽이 활성화됨
		}

		if (!rightButton.gameObject.activeSelf) {
			return 2;  // 오른쪽 성벽이 활성화됨
		}

		return 0;  // 아무 성벽도 활성화되지 않음
	}
}
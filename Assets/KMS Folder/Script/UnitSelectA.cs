using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

public class UnitSelectA : MonoBehaviour {
	public UnitGameManagerA unitGameManager;
	public UnitPlacementManagerA leftPlacementManager;
	public UnitPlacementManagerA rightPlacementManager;
	public UnitSlotManagerA unitSlotManager;
	public Button resetBtn, saveBtn, upgradeBtn;
	public Image[] unitslots;
	public Image[] selectedUnits;

	private List<UnitData> units;

	private IEnumerator Start() {
		yield return new WaitUntil(() => UnitGameManagerA.Instance != null && UnitGameManagerA.Instance.GetUnits() != null);

		if (unitGameManager == null) {
			unitGameManager = UnitGameManagerA.Instance;
		}

		units = unitGameManager.GetUnits();
		resetBtn.onClick.AddListener(ResetUnitSelection);
		saveBtn.onClick.AddListener(SaveUnitSelection);
	}

	private void ResetUnitSelection() {
		string jsonData;
		string savePath = Path.Combine(Application.dataPath, "save", "unitInfo");

		if (leftPlacementManager != null) {
			string leftFilePath = Path.Combine(savePath, "selectedUnitsRight.json");

			if (File.Exists(leftFilePath)) {
				jsonData = File.ReadAllText(leftFilePath);

				foreach (var slotUnit in leftPlacementManager.placementUnit.GetSlotUnitDataList()) {
					unitGameManager.unitPlacementStatus[slotUnit.UnitData] = false;
				}

				unitGameManager.ClearUnitFormation(leftPlacementManager.placementUnit, unitGameManager.leftWallFilePath);
				leftPlacementManager.ResetPlacementSlots(jsonData);
				unitSlotManager.UpdateDraggableStates();
			} else {
				CustomLogger.LogError("selectedUnitsRight.json 파일을 찾을 수 없습니다.");
			}
		}

		if (rightPlacementManager != null) {
			string rightFilePath = Path.Combine(savePath, "selectedUnitsLeft.json");

			if (File.Exists(rightFilePath)) {
				jsonData = File.ReadAllText(rightFilePath);

				foreach (var slotUnit in rightPlacementManager.placementUnit.GetSlotUnitDataList()) {
					unitGameManager.unitPlacementStatus[slotUnit.UnitData] = false;
				}

				unitGameManager.ClearUnitFormation(rightPlacementManager.placementUnit, unitGameManager.rightWallFilePath);
				rightPlacementManager.ResetPlacementSlots(jsonData);
				unitSlotManager.UpdateDraggableStates();
			} else {
				CustomLogger.LogError("selectedUnitsLeft.json 파일을 찾을 수 없습니다.");
			}
		}
	}

	private void SaveUnitSelection() {
		if (leftPlacementManager != null) {
			unitGameManager.SaveUnitFormation(leftPlacementManager.placementUnit, unitGameManager.leftWallFilePath);
			unitSlotManager.UpdateDraggableStates();
		}

		if (rightPlacementManager != null) {
			unitGameManager.SaveUnitFormation(rightPlacementManager.placementUnit, unitGameManager.rightWallFilePath);
			unitSlotManager.UpdateDraggableStates();
		}
	}

	private void upgradeBtnClicked() {
	}
}
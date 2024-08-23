using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class PlacementUnitA : MonoBehaviour {
	[SerializeField] private Transform[] slots;
	private List<SlotUnitData> slotUnitDataList;

	private void Awake() {
		slotUnitDataList = new List<SlotUnitData>();
	}

	public void SavePlacementUnits() {
		slotUnitDataList.Clear();

		for (int i = 0; i < slots.Length; i++) {
			UnitData assignedUnitData = slots[i].GetComponent<UnitDropable>().assignedUnitData;
			UnitDropable dropable = slots[i].GetComponent<UnitDropable>();
			
			CustomLogger.Log("if 들어가기 직전");

			if (assignedUnitData != null) {
				CustomLogger.Log("if들어옴");
				
				int placementStatus = 0;

				if (UnitGameManagerA.Instance.unitPlacementStatus.ContainsKey(assignedUnitData)) {
					placementStatus = UnitGameManagerA.Instance.unitPlacementStatus[assignedUnitData];
					CustomLogger.Log($"unitPlacementStatus contains: {assignedUnitData.name}, Placement: {placementStatus}");
				} else {
					CustomLogger.Log($"unitPlacementStatus does not contain: {assignedUnitData.name}", Color.red);
				}

				int slotIndex = Array.IndexOf(slots, dropable.transform);

				if (placementStatus != 0 && slotIndex != -1) {
					CustomLogger.Log("if안에 if 들옴");
					slotUnitDataList.Add(new SlotUnitData(slotIndex, assignedUnitData, placementStatus));
					CustomLogger.Log($"Saved Slot Index: {slotIndex}, Unit: {assignedUnitData.name}, Placement: {placementStatus}");
				} else {
					CustomLogger.Log($"Skipped saving: {assignedUnitData.name} with Placement: {placementStatus}, SlotIndex: {slotIndex}", Color.yellow);
				}
			}
		}
		
		CustomLogger.Log("for문 탈출");

		CustomLogger.Log("Saved Units: " + slotUnitDataList.Count);
		UnitGameManagerA.Instance.SaveSlotUnitData(slotUnitDataList);
	}

	public void SetSlotUnitDataList(List<SlotUnitData> dataList) {
		if (dataList == null || dataList.Count == 0) {
			CustomLogger.Log("SetSlotUnitDataList received an empty or null dataList.", Color.red);

			return;
		}

		slotUnitDataList = dataList;

		int currentWallStatus = FindFirstObjectByType<StageManager>().GetCurrentWallStatus();

		foreach (var slotUnitData in slotUnitDataList) {
			if (slotUnitData.Placement == currentWallStatus) {
				int slotIndex = slotUnitData.SlotIndex;

				if (slotIndex >= 0 && slotIndex < slots.Length) {
					var slot = slots[slotIndex];
					var dropableComponent = slot.GetComponent<UnitDropable>();

					if (dropableComponent != null) {
						if (slotUnitData.UnitData != null) {
							dropableComponent.assignedUnitData = slotUnitData.UnitData;
							slot.GetComponent<Image>().sprite = slotUnitData.UnitData.UnitImage;
							slot.GetComponent<Image>().color = Color.white;
						} else {
							dropableComponent.assignedUnitData = null;
							slot.GetComponent<Image>().sprite = null;
							slot.GetComponent<Image>().color = Color.white;
						}
					}
				}
			}
		}
	}

	public List<SlotUnitData> GetSlotUnitDataList() {
		return slotUnitDataList ?? new List<SlotUnitData>();
	}

	private void OnDestroy() {
		SavePlacementUnits();
	}
}
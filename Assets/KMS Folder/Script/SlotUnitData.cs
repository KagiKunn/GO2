using System;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class SlotUnitData {
	public int SlotIndex; // 슬롯의 인덱스 번호
	public int ID;
	[NonSerialized] public UnitData UnitData;

	public SlotUnitData() {
	}

	public SlotUnitData(int slotIndex, UnitData unitData) {
		SlotIndex = slotIndex;
		ID = unitData.ID;
		UnitData = unitData;
	}
}

[System.Serializable]
public class SlotUnitDataWrapper {
	public List<SlotUnitData> SlotUnitDataList;

	public SlotUnitDataWrapper() {
	}

	public SlotUnitDataWrapper(List<SlotUnitData> slotUnitDataList) {
		SlotUnitDataList = slotUnitDataList;
	}
}

[System.Serializable]
public class SlotUnitDataListWrapper {
	public List<SlotUnitData> SlotUnitDataList; // JSON의 "SlotUnitDataList" 배열에 대응
}
using System;
using System.Collections.Generic;
using UnityEngine;

// 유닛 이름, 프리팹, 배치 인덱스 저장 예정 -> 이곳에 데이터가 있으면 배치가 되었다는 거

public class SlotUnitData<T1, T2, T3>
{
	public T1 UnitName { get; set; }    // 예: 유닛 이름
	public T2 UnitPrefab { get; set; }  // 예: 유닛 프리팹 (GameObject)
	public T3 SlotIndex { get; set; }   // 예: 배치될 슬롯의 인덱스

	public SlotUnitData(T1 unitName, T2 unitPrefab, T3 slotIndex)
	{
		UnitName = unitName;
		UnitPrefab = unitPrefab;
		SlotIndex = slotIndex;
	}
}

 // public List<SlotUnitData<string, GameObject ,int>> lSelectedUnitList {
 // 	get => L_SelectedUnitList;
 //
 // 	set => L_SelectedUnitList = value;
 // }


//public List<KeyValuePair<string, int>> lUnitList {
// public List<KeyValuePair<int, string>> lAllyUnitList { -> 이걸 유저 슬롯에 불러 와야함? 어느 값들이 저장되어 있는지
// 유닛들 맨 처음 시작 어디서??

[System.Serializable]
public class SlotUnitData
{
	public int SlotIndex; // 슬롯의 인덱스 번호
	public string Name;
	[NonSerialized] public UnitData UnitData;
	public GameObject Prefab;

	public SlotUnitData() { }

	public SlotUnitData(int slotIndex, string name, GameObject prefab)
	{
		SlotIndex = slotIndex;
		Name = UnitData.Name;
		Prefab = UnitData.UnitPrefab;
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
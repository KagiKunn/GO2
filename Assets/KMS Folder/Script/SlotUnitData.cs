using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SlotUnitData
{
    public int SlotIndex; // 슬롯의 인덱스 번호
    public UnitData UnitData; // 유닛 데이터

    public SlotUnitData(int slotIndex, UnitData unitData)
    {
        SlotIndex = slotIndex;
        UnitData = unitData;
    }
}

[System.Serializable]
public class SlotUnitDataWrapper {
    public List<SlotUnitData> SlotUnitDataList;

    public SlotUnitDataWrapper() {}

    public SlotUnitDataWrapper(List<SlotUnitData> slotUnitDataList) {
        SlotUnitDataList = slotUnitDataList;
    }
}
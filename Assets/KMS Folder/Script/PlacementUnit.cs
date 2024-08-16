using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementUnit : MonoBehaviour
{
    [SerializeField] private Transform[] slots;
    private List<UnitData> uds;
    private List<SlotUnitData> slotUnitDataList;
    UnitData ud;

    private void Awake()
    {
        if (slots == null || slots.Length == 0) {
            Debug.LogError("Slots are not assigned in the Inspector or they are empty.");
            return;
        }
        
        slotUnitDataList = new List<SlotUnitData>();
        uds = new List<UnitData>();
        
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) {
                Debug.LogError($"Slot at index {i} is not assigned.");
                continue;
            }
            UnitData assignedUnitData = slots[i].GetComponent<UnitDropable>().assignedUnitData;
            if (assignedUnitData != null)
            {
                slotUnitDataList.Add(new SlotUnitData(i, assignedUnitData)); // 인덱스를 사용하여 저장
                CustomLogger.Log($"Slot Index {i}: {assignedUnitData}");
            }
        }
    }

    public void SavePlacementUnits()
    {
        // 저장 시, 현재 슬롯의 유닛 정보를 갱신
        slotUnitDataList.Clear(); // 이전 데이터를 지우고 새롭게 저장

        for (int i = 0; i < slots.Length; i++)
        {
            UnitData assignedUnitData = slots[i].GetComponent<UnitDropable>().assignedUnitData;
            if (assignedUnitData != null)
            {
                slotUnitDataList.Add(new SlotUnitData(i, assignedUnitData)); // 인덱스를 사용하여 저장
                CustomLogger.Log($"Slot Index {i}: {assignedUnitData}");
            }
        }
    }
    public List<SlotUnitData> GetSlotUnitDataList()
{   
    return slotUnitDataList;
}
    public void SetSlotUnitDataList(List<SlotUnitData> dataList)
    {
        slotUnitDataList = dataList ?? new List<SlotUnitData>();  // null일 경우 빈 리스트로 초기화
    }
}

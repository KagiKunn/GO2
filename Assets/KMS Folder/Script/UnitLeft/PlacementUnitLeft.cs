using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementUnitLeft : MonoBehaviour
{
    [SerializeField] private Transform[] slots;
    private List<SlotUnitData> slotUnitDataList;

    private void Awake()
    {
        slotUnitDataList = new List<SlotUnitData>();

        for (int i = 0; i < slots.Length; i++)
        {
            UnitData assignedUnitData = slots[i].GetComponent<UnitDropable>().assignedUnitData;
            if (assignedUnitData != null)
            {
                slotUnitDataList.Add(new SlotUnitData(i, assignedUnitData)); // 인덱스를 사용하여 저장
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
            }
        }
    }

    public List<SlotUnitData> GetSlotUnitDataList()
    {   
        return slotUnitDataList;
    }

    public void SetSlotUnitDataList(List<SlotUnitData> dataList)
    {
        slotUnitDataList = dataList ?? new List<SlotUnitData>();
    }
}
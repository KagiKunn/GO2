using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementUnitA : MonoBehaviour
{
    [SerializeField] private Transform[] slots;
    private List<SlotUnitData> slotUnitDataList;

    private void Awake()
    {
        slotUnitDataList = new List<SlotUnitData>();
    }

    public void SavePlacementUnits()
    {
        slotUnitDataList.Clear();

        for (int i = 0; i < slots.Length; i++)
        {
            UnitData assignedUnitData = slots[i].GetComponent<UnitDropable>().assignedUnitData;
            if (assignedUnitData != null)
            {
                slotUnitDataList.Add(new SlotUnitData(i, assignedUnitData));
            }
        }
    }

    public List<SlotUnitData> GetSlotUnitDataList()
    {
        return slotUnitDataList ?? new List<SlotUnitData>();
    }
}
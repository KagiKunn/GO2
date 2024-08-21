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
        
        for (int i = 0; i < slots.Length; i++)
        {
            UnitData assignedUnitData = slots[i].GetComponent<UnitDropable>().assignedUnitData;
            UnitDropable dropable = slots[i].GetComponent<UnitDropable>();
            if (assignedUnitData != null)
            {
                int placementStatus = UnitGameManagerA.Instance.unitPlacementStatus.ContainsKey(assignedUnitData)
                    ? UnitGameManagerA.Instance.unitPlacementStatus[assignedUnitData]
                    : 0;
                int slotIndex = Array.IndexOf(slots, dropable.transform); 
                
                slotUnitDataList.Add(new SlotUnitData(slotIndex, assignedUnitData, placementStatus));
                CustomLogger.Log($"Saved Slot Index: {slotIndex}, Unit: {assignedUnitData.name}, Placement: {placementStatus}");
            }
        }
        Debug.Log("Saved Units: " + slotUnitDataList.Count);
        UnitGameManagerA.Instance.SaveSlotUnitData(slotUnitDataList);
    }
    
    public void SetSlotUnitDataList(List<SlotUnitData> dataList)
    {
        slotUnitDataList = dataList ?? new List<SlotUnitData>();

        for (int i = 0; i < slotUnitDataList.Count; i++)
        {
            int slotIndex = slotUnitDataList[i].SlotIndex;
            
            if (slotIndex >= 0 && slotIndex < slots.Length)
            {
                var slot = slots[slotIndex];
                var dropableComponent = slot.GetComponent<UnitDropable>();
                
                if (dropableComponent != null)
                {
                    dropableComponent.assignedUnitData = slotUnitDataList[i].UnitData;
                    slot.GetComponent<Image>().sprite = slotUnitDataList[i].UnitData.UnitImage;
                    slot.GetComponent<Image>().color = Color.white;
                }
            }
        }
    }

    public List<SlotUnitData> GetSlotUnitDataList()
    {
        return slotUnitDataList ?? new List<SlotUnitData>();
    }
    
    private void OnDestroy()
    {
        SavePlacementUnits();
    }
    
}
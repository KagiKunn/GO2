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
            UnitDropable dropable = slots[i].GetComponent<UnitDropable>();
            
            if (assignedUnitData != null)
            {
                int placementStatus = 0;
                if (UnitGameManagerA.Instance.unitPlacementStatus.ContainsKey(assignedUnitData))
                {
                    placementStatus = UnitGameManagerA.Instance.unitPlacementStatus[assignedUnitData];
                    CustomLogger.Log($"unitPlacementStatus contains: {assignedUnitData.name}, Placement: {placementStatus}");
                }
                else
                {
                    CustomLogger.Log($"unitPlacementStatus does not contain: {assignedUnitData.name}", Color.red);
                }
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
        if (dataList == null || dataList.Count == 0)
        {
            CustomLogger.Log("SetSlotUnitDataList received an empty or null dataList.", Color.red);
            return;
        }

        slotUnitDataList = dataList;

        for (int i = 0; i < slotUnitDataList.Count; i++)
        {
            int slotIndex = slotUnitDataList[i].SlotIndex;
        
            if (slotIndex >= 0 && slotIndex < slots.Length)
            {
                var slot = slots[slotIndex];
                var dropableComponent = slot.GetComponent<UnitDropable>();
            
                if (dropableComponent != null)
                {
                    if (slotUnitDataList[i].UnitData != null)
                    {
                        dropableComponent.assignedUnitData = slotUnitDataList[i].UnitData;
                        slot.GetComponent<Image>().sprite = slotUnitDataList[i].UnitData.UnitImage;
                        slot.GetComponent<Image>().color = Color.white;
                    }
                    else
                    {
                        dropableComponent.assignedUnitData = null;
                        slot.GetComponent<Image>().sprite = null;
                        slot.GetComponent<Image>().color = Color.white;
                    }
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
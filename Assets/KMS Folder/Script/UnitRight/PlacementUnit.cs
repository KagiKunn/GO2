using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementUnit : MonoBehaviour
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
        CustomLogger.Log(slotUnitDataList, "blue");
        return slotUnitDataList;
    }

    public void SetSlotUnitDataList(List<SlotUnitData> dataList)
    {
        slotUnitDataList = dataList ?? new List<SlotUnitData>();
        UpdateSlotImages();
    }
    
    private void UpdateSlotImages()
    {
        foreach (var slotUnit in slotUnitDataList)
        {
            if (slotUnit.SlotIndex >= 0 && slotUnit.SlotIndex < slots.Length)
            {
                var slot = slots[slotUnit.SlotIndex];
                var image = slot.GetComponent<Image>();
                if (image != null && slotUnit.UnitData != null)
                {
                    image.sprite = slotUnit.UnitData.UnitImage;
                    image.color = Color.white;
                    image.enabled = true;
                }
            }
        }
    }
}

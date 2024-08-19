using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPlacementManager : MonoBehaviour
{
    public Transform contentParent;
    public PlacementUnit placementUnit;
    private List<Image> placementImages = new List<Image>();
    private List<SlotUnitData> placementUnits;

    private void Awake()
    {
        for (int i = 0; i < contentParent.childCount; i++)
        {
            Transform slot = contentParent.GetChild(i);
            Image placementImage = slot.GetComponent<Image>();

            if (placementImage != null)
            {
                placementImages.Add(placementImage);
            }
            
        }
    }

    private void Start()
    {
        UnitGameManagerLeft.Instance.LoadUnitFormation();
        AssignSavedUnitsToSlots();
    }
    
    private void AssignSavedUnitsToSlots()
    {
        List<SlotUnitData> savedUnits = placementUnit.GetSlotUnitDataList();
        
        foreach (var slotUnitData in savedUnits)
        {
            int slotIndex = slotUnitData.SlotIndex;
            if (slotIndex >= 0 && slotIndex < placementImages.Count)
            {
                if (slotUnitData.UnitData != null)
                {
                    SetUnitData(placementImages[slotIndex], slotUnitData.UnitData);
                }
                else
                {
                    SetUnitData(placementImages[slotIndex], null);
                }
            }
            else
            {
                Debug.LogError($"Invalid Slot Index or Null Image Reference: {slotUnitData.SlotIndex}");
            }
        }
    }

    private void SetUnitData(Image unitImage, UnitData data)
    {
        if (data != null && data.UnitImage != null)
        {
            unitImage.sprite = data.UnitImage;
            unitImage.color = Color.white;
            unitImage.enabled = true;
        }
    }

    public void ResetPlacementSlots()
    {
        foreach (var placementImage in placementImages)
        {
            var dropableComponent = placementImage.GetComponent<UnitDropable>();
            placementImage.sprite = null;
            dropableComponent.assignedUnitData = null;
        }
    }
}

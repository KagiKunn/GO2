using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPlacementManagerA : MonoBehaviour
{
    public Transform contentParent;
    public PlacementUnitA placementUnit;
    private List<Image> placementImages = new List<Image>();

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
        UnitGameManagerA.Instance.LoadUnitFormation(placementUnit, UnitGameManagerA.Instance.GetFilePath(placementUnit));
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
                SetUnitData(placementImages[slotIndex], slotUnitData.UnitData);
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
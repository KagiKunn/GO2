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
        List<SlotUnitData> savedUnits = UnitGameManagerA.Instance.LoadSlotUnitData();
        if (savedUnits != null && savedUnits.Count > 0)
        {
            if (savedUnits.Count > 0)
            {
                placementUnit.SetSlotUnitDataList(savedUnits);
            }
        }
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
            else
            {
                placementImages[slotIndex].sprite = null;
                placementImages[slotIndex].color = Color.white;
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
            if (dropableComponent != null && dropableComponent.assignedUnitData != null)
            {
                UnitGameManagerA.Instance.SaveUnitPlacement(-1, dropableComponent.assignedUnitData, 0);
                dropableComponent.assignedUnitData = null;
            }
            placementImage.sprite = null;
        }
    }
    
}

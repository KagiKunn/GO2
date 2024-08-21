using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitSlotManagerA : MonoBehaviour
{
    public Transform contentParent;
    private List<UnitData> userUnits;
    private List<Image> unitImages = new List<Image>();
    private List<Graphic> unitSlots = new List<Graphic>();
    private List<UnitDraggable> unitDraggables = new List<UnitDraggable>();

    private void Awake()
    {
        for (int i = 0; i < contentParent.childCount; i++)
        {
            Transform slot = contentParent.GetChild(i);
            Image unitImage = slot.Find("Unit").GetComponent<Image>();
            UnitDraggable unitDraggable = slot.Find("Unit").GetComponent<UnitDraggable>();
            Graphic slotGraphic = slot.GetComponent<Graphic>();
            
            if (unitImage != null && unitDraggable != null)
            {
                unitImages.Add(unitImage);
                unitDraggables.Add(unitDraggable);
                unitSlots.Add(slotGraphic);
            }
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => 
            UnitGameManagerA.Instance != null && UnitGameManagerA.Instance.GetUnits() != null);
        
        userUnits = UnitGameManagerA.Instance.GetUnits();
        AssignUnitsToSlots();
        UpdateDraggableStates();
    }

    private void AssignUnitsToSlots()
    {
        int count = Mathf.Min(unitImages.Count, unitDraggables.Count, userUnits.Count);

        for (int i = 0; i < count; i++)
        {
            SetUnitData(unitImages[i], unitDraggables[i], userUnits[i]);
        }

        for (int i = count; i < unitImages.Count; i++)
        {
            SetUnitData(unitImages[i], unitDraggables[i], null);
        }
    }

    private void SetUnitData(Image unitImage, UnitDraggable unitDraggable, UnitData data)
    {
        if (data != null && data.UnitImage != null)
        {
            unitImage.sprite = data.UnitImage;
            unitImage.color = Color.white;
            unitImage.enabled = true;

            unitDraggable.unitData = data;
        }
        else
        {
            unitImage.sprite = null;
            unitImage.enabled = false;
            unitDraggable.unitData = null;
        }
    }

    public void UpdateDraggableStates()
    {
        int currentWallStatus = FindFirstObjectByType<StageManager>().GetCurrentWallStatus();

        foreach (var draggable in unitDraggables)
        {
            if (draggable == null || draggable.unitData == null)
            {
                continue;
            }

            int placementStatus = UnitGameManagerA.Instance.unitPlacementStatus[draggable.unitData];

            if (placementStatus != 0 && placementStatus == currentWallStatus)
            {
                draggable.isDropped = true;
                draggable.enabled = false;
                draggable.GetComponent<CanvasGroup>().blocksRaycasts = false;
                draggable.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
            }
            else
            {
                draggable.isDropped = false;
                draggable.enabled = true;
                draggable.GetComponent<CanvasGroup>().blocksRaycasts = true;
                draggable.GetComponent<Image>().color = Color.white;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectA : MonoBehaviour
{
    public UnitGameManagerA unitGameManager;
    public UnitPlacementManagerA leftPlacementManager;
    public UnitPlacementManagerA rightPlacementManager;
    public UnitSlotManagerA unitSlotManager;
    public Button resetBtn, saveBtn, upgradeBtn;
    public Image[] unitslots;
    public Image[] selectedUnits;
    
    private List<UnitData> units;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => UnitGameManagerA.Instance != null && UnitGameManagerA.Instance.GetUnits() != null);

        if (unitGameManager == null)
        {
            unitGameManager = UnitGameManagerA.Instance;
        }

        units = unitGameManager.GetUnits();
        resetBtn.onClick.AddListener(ResetUnitSelection);
        saveBtn.onClick.AddListener(SaveUnitSelection);
    }

    private void ResetUnitSelection()
    {
        if (leftPlacementManager != null)
        {
            foreach (var slotUnit in leftPlacementManager.placementUnit.GetSlotUnitDataList())
            {
                unitGameManager.unitPlacementStatus[slotUnit.UnitData] = false;
            }
            unitGameManager.ClearUnitFormation(leftPlacementManager.placementUnit, unitGameManager.leftWallFilePath);
            leftPlacementManager.ResetPlacementSlots();
            unitSlotManager.UpdateDraggableStates();
        }

        if (rightPlacementManager != null)
        {
            foreach (var slotUnit in rightPlacementManager.placementUnit.GetSlotUnitDataList())
            {
                unitGameManager.unitPlacementStatus[slotUnit.UnitData] = false;
            }
            unitGameManager.ClearUnitFormation(rightPlacementManager.placementUnit, unitGameManager.rightWallFilePath);
            rightPlacementManager.ResetPlacementSlots();
            unitSlotManager.UpdateDraggableStates();
        }

    }


    private void SaveUnitSelection()
    {
        if (leftPlacementManager != null)
        {
            unitGameManager.SaveUnitFormation(leftPlacementManager.placementUnit, unitGameManager.leftWallFilePath);
            unitSlotManager.UpdateDraggableStates();
        }

        if (rightPlacementManager != null)
        {
            unitGameManager.SaveUnitFormation(rightPlacementManager.placementUnit, unitGameManager.rightWallFilePath);
            unitSlotManager.UpdateDraggableStates();
        }
        
    }

    private void upgradeBtnClicked()
    {
        
    }
}
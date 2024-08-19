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
        unitSlotManager.UpdateDraggableStates();

        resetBtn.onClick.AddListener(ResetUnitSelection);
        saveBtn.onClick.AddListener(SaveUnitSelection);
        
        unitSlotManager.UpdateDraggableStates();
    }

    private void ResetUnitSelection()
    {
        if (leftPlacementManager != null)
        {
            unitGameManager.ClearUnitFormation(leftPlacementManager.placementUnit, unitGameManager.leftWallFilePath);
            leftPlacementManager.ResetPlacementSlots();
        }

        if (rightPlacementManager != null)
        {
            unitGameManager.ClearUnitFormation(rightPlacementManager.placementUnit, unitGameManager.rightWallFilePath);
            rightPlacementManager.ResetPlacementSlots();
        }
        unitSlotManager.ResetUnitDrag();
    }

    private void SaveUnitSelection()
    {
        if (leftPlacementManager != null)
        {
            unitGameManager.SaveUnitFormation(leftPlacementManager.placementUnit, unitGameManager.leftWallFilePath);
        }

        if (rightPlacementManager != null)
        {
            unitGameManager.SaveUnitFormation(rightPlacementManager.placementUnit, unitGameManager.rightWallFilePath);
        }
        
        unitSlotManager.UpdateDraggableStates();
    }

    private void upgradeBtnClicked()
    {
        
    }
}
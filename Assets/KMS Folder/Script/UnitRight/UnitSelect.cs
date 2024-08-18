using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelect : MonoBehaviour
{
    public UnitGameManager unitGameManager;
    public UnitPlacementManager unitPlacementManager;
    public UnitSlotManager unitSlotManager;
    public Button resetBtn, saveBtn, upgradeBtn;
    public Image[] unitslots;
    public Image[] selectedUnits;
    
    private List<UnitData> units;

    private void Start()
    {
        units = UnitGameManager.Instance.GetUnits();
        resetBtn.onClick.AddListener(ResetUnitSelection);
        saveBtn.onClick.AddListener(SaveUnitSelection);
    }

    private void ResetUnitSelection()
    {
        UnitGameManager.Instance.ClearUnitFormation();
        unitPlacementManager.ResetPlacementSlots(); 
        unitSlotManager.ResetUnitDrag();
    }

    private void SaveUnitSelection()
    {
        UnitGameManager.Instance.SaveUnitFormation();
    }

    private void upgradeBtnClicked()
    {
        
    }
    
}
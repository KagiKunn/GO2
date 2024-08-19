using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class UnitSelectLeft : MonoBehaviour
{
    public UnitGameManagerLeft unitGameManager;
    public UnitPlacementManagerLeft unitPlacementManagerLeft;
    public UnitSlotManagerLeft unitSlotManager;
    public Button resetBtn, saveBtn, upgradeBtn;
    public Image[] unitslots;
    public Image[] selectedUnits;
    
    private List<UnitData> units;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => UnitGameManagerLeft.Instance != null && UnitGameManagerLeft.Instance.GetUnits() != null);
        if (unitGameManager == null)
        {
            unitGameManager = FindFirstObjectByType<UnitGameManagerLeft>();
        }

        unitSlotManager.UpdateDraggableStates();
        
        units = UnitGameManagerLeft.Instance.GetUnits();
        resetBtn.onClick.AddListener(ResetUnitSelection);
        saveBtn.onClick.AddListener(SaveUnitSelection);
        
    }

    private void ResetUnitSelection()
    {
        UnitGameManagerLeft.Instance.ClearUnitFormation();
        unitPlacementManagerLeft.ResetPlacementSlots(); 
        unitSlotManager.ResetUnitDrag();
    }

    private void SaveUnitSelection()
    {
        UnitGameManagerLeft.Instance.SaveUnitFormation();
    }
    
    private void upgradeBtnClicked()
    {
        // 업그레이드 버튼에 대한 로직
    }
}
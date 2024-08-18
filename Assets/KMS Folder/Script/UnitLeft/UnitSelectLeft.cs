using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UnitSelectLeft : MonoBehaviour
{
    public UnitGameManagerLeft unitGameManager;
    public UnitPlacementManagerLeft unitPlacementManagerLeft;  // 왼쪽 성벽에 대한 PlacementManager
    public UnitSlotManagerLeft unitSlotManager;
    public Button resetBtn, saveBtn, upgradeBtn;
    public Image[] unitslots;
    public Image[] selectedUnits;
    
    private List<UnitData> units;

    private void Start()
    {
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
        UnitGameManagerLeft.Instance.ClearUnitFormation();  // 왼쪽 성벽 유닛 배치 정보 초기화
        unitPlacementManagerLeft.ResetPlacementSlots(); 
        unitSlotManager.ResetUnitDrag();
    }

    private void SaveUnitSelection()
    {
        UnitGameManagerLeft.Instance.SaveUnitFormation();  // 왼쪽 성벽 유닛 배치 정보 저장
    }
    
    private void upgradeBtnClicked()
    {
        // 업그레이드 버튼에 대한 로직
    }
}
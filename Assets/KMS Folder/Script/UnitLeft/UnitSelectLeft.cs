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
        units = UnitGameManagerLeft.Instance.GetUnits();
        resetBtn.onClick.AddListener(ResetUnitSelection);
        saveBtn.onClick.AddListener(SaveUnitSelection);
        // UpdateDraggableStates(); 
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
    
    private void UpdateDraggableStates()
    {
        List<UnitData> rightSelectedUnits = UnitGameManager.Instance.GetSelectedUnits();  // 오른쪽 성벽의 배치 유닛
        List<UnitData> leftSelectedUnits = UnitGameManagerLeft.Instance.GetSelectedUnits();  // 왼쪽 성벽의 배치 유닛

        foreach (var draggable in unitSlotManager.unitDraggables)
        {
            if (rightSelectedUnits.Contains(draggable.unitData) || leftSelectedUnits.Contains(draggable.unitData))
            {
                draggable.SetDraggable(false); // 이미 배치된 유닛은 드래그 불가
                draggable.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f); // 어둡게 표시
            }
            else
            {
                draggable.SetDraggable(true); // 배치되지 않은 유닛만 드래그 가능
                draggable.GetComponent<Image>().color = Color.white; // 기본 색상으로 표시
            }
        }
    }
    private void upgradeBtnClicked()
    {
        // 업그레이드 버튼에 대한 로직
    }
}

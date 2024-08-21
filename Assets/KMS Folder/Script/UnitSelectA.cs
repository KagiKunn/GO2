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
    public Button leftWallBtn, rightWallBtn;
    public GameObject leftWallContent, rightWallContent;
    public Image[] unitslots;
    public Image[] selectedUnits;

    private List<UnitData> units;

    private IEnumerator Start() {
        yield return new WaitUntil(() => UnitGameManagerA.Instance != null && UnitGameManagerA.Instance.GetUnits() != null);

        if (unitGameManager == null) {
            unitGameManager = UnitGameManagerA.Instance;
        }

        units = unitGameManager.GetUnits();

        resetBtn.onClick.AddListener(ResetUnitSelection);
        saveBtn.onClick.AddListener(SaveUnitSelection);
        leftWallBtn.onClick.AddListener(ShowLeftWall);
        rightWallBtn.onClick.AddListener(ShowRightWall);
    }

    private void ShowLeftWall()
    {
        leftWallContent.SetActive(true);
        rightWallContent.SetActive(false);
    }

    private void ShowRightWall()
    {
        rightWallContent.SetActive(true);
        leftWallContent.SetActive(false);
    }

    private void ResetUnitSelection()
    {
        if (leftWallContent.activeSelf)
        {
            ResetWallPlacement(1);
        }
        else if (rightWallContent.activeSelf)
        {
            ResetWallPlacement(2);
        }
    }

    private void ResetWallPlacement(int wallIndex)
    {
        if (wallIndex == 1)
        {
            leftPlacementManager.ResetPlacementSlots();
        }
        else if (wallIndex == 2)
        {
            rightPlacementManager.ResetPlacementSlots();
        }

        unitSlotManager.UpdateDraggableStates();
    }

    private void SaveUnitSelection()
    {
        if (leftWallContent.activeSelf)
        {
            leftPlacementManager.SaveCurrentPlacements(true);
        }
        else if (rightWallContent.activeSelf)
        {
            rightPlacementManager.SaveCurrentPlacements(false);
        }

        unitSlotManager.UpdateDraggableStates();
    }
}

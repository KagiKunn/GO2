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
    public Button resetBtn;
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
        int currentWallStatus = FindFirstObjectByType<StageManager>().GetCurrentWallStatus();
        unitSlotManager.ResetWallPlacement(currentWallStatus);
    }
    
}

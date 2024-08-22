using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StageManager : MonoBehaviour
{
    public GameObject[] leftStageUnits;
    public GameObject[] rightStageUnits;
    public Button leftButton;
    public Button rightButton;

    private UnitGameManagerA unitGameManager;
    private UnitSlotManagerA unitSlotManager;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => UnitGameManagerA.Instance != null);

        unitGameManager = UnitGameManagerA.Instance;
        unitSlotManager = FindFirstObjectByType<UnitSlotManagerA>();

        leftButton.onClick.AddListener(ShowLeftStage);
        rightButton.onClick.AddListener(ShowRightStage);

        CustomLogger.Log("StageManagement 스타트 메서드 활성화", "green");
    }

    private void ShowLeftStage()
    {
        
        SaveCurrentWallPlacement();
        
        SetStageActive(leftStageUnits, true);
        SetStageActive(rightStageUnits, false);
        
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(true);

    }

    private void ShowRightStage()
    {
        
        SaveCurrentWallPlacement();
        
        SetStageActive(leftStageUnits, false);
        SetStageActive(rightStageUnits, true);

        rightButton.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(true);
    }

    private void SetStageActive(GameObject[] stageUnits, bool isActive)
    {
        foreach (var unit in stageUnits)
        {
            unit.SetActive(isActive);
        }
    }

    public int GetCurrentWallStatus()
    {
        if (!leftButton.gameObject.activeSelf)
        {
            return 1;
        }
        if (!rightButton.gameObject.activeSelf)
        {
            return 2;
        }
        return 0;
    }
    
    private void SaveCurrentWallPlacement()
    {
        PlacementUnitA placementUnit = FindAnyObjectByType<PlacementUnitA>();
        if (placementUnit != null)
        {
            placementUnit.SavePlacementUnits();
            CustomLogger.Log("현재 성벽 데이터를 저장했습니다.", Color.green);
        }
        else
        {
            CustomLogger.Log("PlacementUnitA를 찾을 수 없습니다.", Color.red);
        }
    }
}
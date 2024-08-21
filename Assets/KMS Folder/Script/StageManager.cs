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
        // UnitGameManagerA.Instance가 초기화될 때까지 대기
        yield return new WaitUntil(() => UnitGameManagerA.Instance != null);

        unitGameManager = UnitGameManagerA.Instance;
        unitSlotManager = FindFirstObjectByType<UnitSlotManagerA>();

        leftButton.onClick.AddListener(ShowLeftStage);
        rightButton.onClick.AddListener(ShowRightStage);

        CustomLogger.Log("StageManagement 스타트 메서드 활성화", "green");
    }

    private void ShowLeftStage()
    {
        SetStageActive(leftStageUnits, true);
        SetStageActive(rightStageUnits, false);
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(true);

    }

    private void ShowRightStage()
    {
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
            CustomLogger.Log("왼쪽 성벽", "Red");
            return 1;
        }
        if (!rightButton.gameObject.activeSelf)
        {
            CustomLogger.Log("오른쪽 성벽", "Blue");
            return 2;
        }
        Debug.LogError("StageManger가 잘못 된 듯");
        return 0;
    }
}
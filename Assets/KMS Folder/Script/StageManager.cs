using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StageManager : MonoBehaviour
{
    public Button leftButton;
    public Button rightButton;

    private UnitGameManagerA unitGameManager;
    private UnitSlotManagerA unitSlotManager;
    
    public GameObject[] slots;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => UnitGameManagerA.Instance != null);

        unitGameManager = UnitGameManagerA.Instance;
        unitSlotManager = FindFirstObjectByType<UnitSlotManagerA>();

        leftButton.onClick.AddListener(ShowLeftStage);
        rightButton.onClick.AddListener(ShowRightStage);
        
        ShowLeftStage(); 

        CustomLogger.Log("StageManagement 스타트 메서드 활성화", "green");
    }

    private void ShowLeftStage()
    {
        
        SetSlotVisibility(0, 13);
        
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(true);

    }

    private void ShowRightStage()
    {
        
        SetSlotVisibility(14, 27);

        rightButton.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(true);
    }

    private void SetSlotVisibility(int start, int end)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetActive(i >= start && i <= end);
        }
    }
    
}
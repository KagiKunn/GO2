using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelect : MonoBehaviour
{
    public UnitGameManager unitGameManager;
    public Button resetBtn, saveBtn, upgradeBtn;
    public Image[] unitslots;
    public Image[] selectedUnits;
    
    private List<UnitData> units;

    private void Awake()
    {
    }

    private void Start()
    {
        units = UnitGameManager.Instance.GetUnits();
        resetBtn.onClick.AddListener(ResetUnitSelection);
        saveBtn.onClick.AddListener(SaveUnitSelection);
    }

    private void ResetUnitSelection()
    {
        foreach (var dropZone in selectedUnits)
        {
            Image droppedImage = dropZone.GetComponent<Image>();
            droppedImage = null;
        }
        UnitGameManager.Instance.ClearUnitFormation();
    }

    private void SaveUnitSelection()
    {
        CustomLogger.Log("버튼 눌림");
        UnitGameManager.Instance.SaveUnitFormation();
    }

    private void upgradeBtnClicked()
    {
        
    }
    
}
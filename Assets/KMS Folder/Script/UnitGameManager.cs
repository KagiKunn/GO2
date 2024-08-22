using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitGameManager : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent;

    private List<KeyValuePair<string, int>> userUnits; // 유저 구매 유닛 및 가진 유닛들
    // private List<SlotUnitData<string, GameObject ,int>> selectedUnits;

    private void Start()
    {
        LoadUserUnit();
        DisplayUnits();
    }

    private void LoadUserUnit()
    {
        userUnits = PlayerLocalManager.Instance.lUnitList;
        if (userUnits == null)
        {
            CustomLogger.Log("NULL");
        }
    }
    
    private void DisplayUnits()
    {
        foreach (var unitData in userUnits)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotParent);

            TextMeshProUGUI unitName = newSlot.GetComponentInChildren<TextMeshProUGUI>();
            if (unitName != null)
            {
                unitName.text = unitData.Key;
            }
            else
            {
                Debug.LogError("슬롯에 TextMeshProUGUI 컴포넌트가 없습니다.");
            }
            
        }
    }
}

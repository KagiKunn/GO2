using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitGameManager : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent;

    private List<KeyValuePair<int, string>> userUnits; // 유저 구매 유닛 및 가진 유닛들
    private List<SlotUnitData<string, GameObject ,int>> selectedUnits;

    private void Start()
    {
        LoadUserUnit();
        DisplayUnits();
    }

    private void LoadUserUnit()
    {
        userUnits = PlayerLocalManager.Instance.lAllyUnitList;
    }
    
    private void DisplayUnits()
    {
        foreach (var unitData in userUnits)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotParent);

            TextMeshProUGUI unitName = newSlot.GetComponentInChildren<TextMeshProUGUI>();
            if (unitName != null)
            {
                unitName.text = unitData.Value;
            }
            else
            {
                Debug.LogError("슬롯에 TextMeshProUGUI 컴포넌트가 없습니다.");
            }
            
        }
    }

    private void SaveUnitList(string name, GameObject prefab, int index)
    {
        // 성벽 버튼에서 넘어오기(StageManager) / 배치할때 
        
        
        
        // SlotUnitData<string, GameObject ,int> slotUnit = new SlotUnitData<string, GameObject ,int>(유닛 이름, 프리팹, slotindex);
        // selectedUnits.Add(slotUnit);

        // PlayerLocalManager.Instance.l_SelectedUnitList = selectedUnits;
        PlayerLocalManager.Instance.Save();
    }
    
    
    
}



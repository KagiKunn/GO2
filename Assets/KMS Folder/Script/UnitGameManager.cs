using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitGameManager : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent;

    public GameObject[] Prefabs; // 리소스 폴더 내 유닛 프리팹
    public GameObject[] Slot; // Unit 1. 2. 3. 4 같은 Dropable 이미지 컴퍼넌트 가진 애들
    
    private List<KeyValuePair<string, int>> userUnits; // 유저 구매 유닛 및 가진 유닛들
    public List<KeyValuePair<int, string>> selectedUnits;
    
    private List<UnitDraggable> unitDraggables = new List<UnitDraggable>();

    private void Start()
    {
        LoadUserUnit();
        
        if (selectedUnits != null)
        {
            DisplayPrefab();
        }
        else
        {
            CustomLogger.Log("매니저 스타트인데 데이터가 없음.");
        }
        DisplayUnitsList();
    }

    private void LoadUserUnit()
    {
        userUnits = PlayerLocalManager.Instance.lUnitList;

        CustomLogger.Log("LoadUserUnit start");
        if (PlayerLocalManager.Instance.lAllyUnitList != null)
        {
            CustomLogger.Log("null 아님");
            selectedUnits = PlayerLocalManager.Instance.lAllyUnitList;
        }
        else
        {
            selectedUnits = new List<KeyValuePair<int, string>>();
        }
    }
    
    private void DisplayUnitsList()
    {
        foreach (var unitData in userUnits)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotParent);
            TextMeshProUGUI unitName = newSlot.GetComponentInChildren<TextMeshProUGUI>();
            
            if (unitName != null)
            {
                unitName.text = unitData.Key;
                CustomLogger.Log(unitData.Key+ unitData.Value, Color.cyan);
            }
            else
            {
                Debug.LogWarning("슬롯에 TextMeshProUGUI 컴포넌트 없음.");
            }
            
            UnitDraggable unitDraggable = newSlot.GetComponent<UnitDraggable>();
            if (unitDraggable != null)
            {
                unitDraggable.unitName = unitData.Key;
                unitDraggables.Add(unitDraggable);

                // 유닛이 이미 배치된 상태인지 확인하고, 배치된 경우 드래그 불가 설정
                if (IsUnitAlreadyPlaced(unitData.Value)) // 유닛이 이미 배치된 상태인지 확인
                {
                    unitDraggable.SetDraggable(false);
                    unitDraggable.GetComponent<CanvasGroup>().alpha = 0.6f;
                }
            }
        }
    }
    
    private bool IsUnitAlreadyPlaced(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < slotParent.childCount;
    }
    
    public void DisplayPrefab()
    {
        Prefabs = Resources.LoadAll<GameObject>("Defense/Unit");

        if (selectedUnits != null)
        {
            foreach (var unitData in selectedUnits)
            {
                int slotIndex = unitData.Key;
                string unitName = unitData.Value;

                // 이름과 일치하는 프리팹 찾기
                GameObject prefabname = FindPrefabByName(unitName);

                if (prefabname != null)
                {
                    GameObject prefabObject = Instantiate(prefabname, Slot[slotIndex].transform); // 부모 객체
                    RectTransform prefab = prefabObject.GetComponent<RectTransform>(); // 드래그 했을때 그 유닛의 이름을 바탕으로 뽑아와야함
                    RectTransform slotRect = Slot[slotIndex].GetComponent<RectTransform>();

                    // 프리팹 크기 = 슬롯 크기
                    prefab.sizeDelta = slotRect.sizeDelta;
                    prefab.localScale = new Vector3(200, 200, 1);

                    // 프리팹 위치 설정
                    prefab.anchoredPosition = new Vector2(0, -50);
                }
                else
                {
                    CustomLogger.Log("유닛 이름이랑 프리팹 다르다캄", Color.yellow);
                }
            }
        }
    }
    
    private GameObject FindPrefabByName(string unitName)
    {
        Prefabs = Resources.LoadAll<GameObject>("Defense/Unit");
        foreach (GameObject prefab in Prefabs)
        {
            if (prefab.name == unitName)
            {
                return prefab;
            }
        }
        CustomLogger.Log("일치하는 프리팹 없음", Color.yellow);
        return null;
    }

    public void ResetList()
    {
        selectedUnits = new List<KeyValuePair<int, string>>();
        PlayerLocalManager.Instance.lAllyUnitList = selectedUnits;
        PlayerLocalManager.Instance.Save();
        
        // 기존에 배치된 프리팹 제거 (필요한 경우)
        foreach (Transform slot in slotParent)
        {
            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }
        }

        foreach (var unitDraggable in unitDraggables)
        {
            unitDraggable.SetDraggable(true);
            unitDraggable.GetComponent<CanvasGroup>().alpha = 1f;
        }
    }
}



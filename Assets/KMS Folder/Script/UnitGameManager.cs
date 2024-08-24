using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UnitGameManager : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent;
    public Transform placementParent;
    public UnitDropable unintDropable;

    public GameObject[] Prefabs; // 리소스 폴더 내 유닛 프리팹
    public GameObject[] Slot; // 유닛 배치 슬롯
    
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

        if (PlayerLocalManager.Instance.lAllyUnitList != null)
        {
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
            }
        }
        foreach (var unitDraggable in unitDraggables)
        {
            if (unitDraggable.unitName == "Default")
            {
                continue;
            }

            int placedCount = selectedUnits.Count(unit => unit.Value == unitDraggable.unitName);

            if (placedCount > 0)
            {
                foreach (var otherSlot in unitDraggables.Where(ud => ud.unitName == unitDraggable.unitName))
                {
                    if (placedCount <= 0)
                        break;

                    if (otherSlot.gameObject.activeSelf)
                    {
                        otherSlot.gameObject.SetActive(false);
                        placedCount--;
                    }
                }
            }
        }
    }
    

    public int CountUnitsAlreadyPlaced(string unitName)
    {
        return selectedUnits.Count(unit => unit.Value == unitName);
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

                GameObject prefabname = FindPrefabByName(unitName);

                if (prefabname != null)
                {
                    unintDropable.RemoveExistingPrefab(slotIndex);
                    GameObject prefabObject = Instantiate(prefabname, Slot[slotIndex].transform); // 부모 객체
                    RectTransform prefab = prefabObject.GetComponent<RectTransform>(); // 드래그 했을때 그 유닛의 이름을 바탕
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
    
    public GameObject FindPrefabByName(string unitName)
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
    
    public void RemoveUnitFromList(string unitName)
    {
        foreach (Transform child in slotParent)
        {
            TextMeshProUGUI unitText = child.GetComponentInChildren<TextMeshProUGUI>();
        
            // 이미 비활성화된 슬롯은 건너뛰고, 활성화된 슬롯을 비활성화
            if (unitText != null && unitText.text == unitName && child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(false);  // 슬롯 비활성화
                CustomLogger.Log($"{unitName} 지워짐", Color.yellow);
                break;  // 첫 번째로 일치하는 활성화된 슬롯을 비활성화한 후 루프 종료
            }
        }
    }

    public void ResetList()
    {
        selectedUnits = new List<KeyValuePair<int, string>>();
        PlayerLocalManager.Instance.lAllyUnitList = selectedUnits;
        PlayerLocalManager.Instance.Save();

        foreach (Transform slot in placementParent)
        {
            foreach (Transform child in slot)
            {
                if (child.name.EndsWith("(Clone)"))
                {
                    Destroy(child.gameObject);
                }
            }
        }
        foreach (Transform child in slotParent)
        {
            child.gameObject.SetActive(true);
        }
        
        foreach (var unitDraggable in unitDraggables)
        {
            unitDraggable.SetDraggable(true);
            var canvasGroup = unitDraggable.GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = true;
        }
    }
    
    public void SaveDefaultUnitData()
    {
        HashSet<int> occupiedSlots = new HashSet<int>();
        foreach (var unit in selectedUnits)
        {
            occupiedSlots.Add(unit.Key);
        }

        for (int i = 0; i < 28; i++)
        {
            if (!occupiedSlots.Contains(i))
            {
                KeyValuePair<int, string> defaultUnitData = new KeyValuePair<int, string>(i, "Default");
                CustomLogger.Log(defaultUnitData.Key + " " + defaultUnitData.Value, Color.magenta);
                selectedUnits.Add(defaultUnitData);
            }
        }
        PlayerLocalManager.Instance.lAllyUnitList = selectedUnits;
        PlayerLocalManager.Instance.Save();
    }
}




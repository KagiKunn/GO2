using System.Collections.Generic;
using System.Linq;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitGameManager : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent;
    public Transform placementParent;
    public UnitDropable unintDropable;

    public GameObject[] Prefabs; // 리소스 폴더 내 유닛 프리팹
    public GameObject[] Slot; // 유닛 배치 슬롯
    public Transform panelTransform;

    private List<KeyValuePair<string, int>> userUnits; // 유저 구매 유닛 및 가진 유닛들
    public List<KeyValuePair<int, string>> selectedUnits;
    private List<UnitDraggable> unitDraggables = new List<UnitDraggable>();

    private void Awake()
    {
        LoadUserUnit();

        if (selectedUnits != null)
        {
            DisplayPrefab();
        }
        DisplayUnitsList();
        UpdateUnitsList();
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
        Sprite[] prefabImages = Resources.LoadAll<Sprite>("Image/Unit");
        foreach (var unitData in userUnits)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotParent);
            Image unitImage = newSlot.GetComponentInChildren<Image>();

            if (unitImage != null)
            {
                foreach (var prefabImage in prefabImages)
                {
                    string spriteNameWithoutSuffix = prefabImage.name.Split('_')[0];
                    if (spriteNameWithoutSuffix == unitData.Key)
                    {
                        unitImage.sprite = prefabImage;
                        break;
                    }
                }
            }
            else
            {
                Debug.LogWarning("슬롯에 Image 컴포넌트 없음.");
            }

            UnitDraggable unitDraggable = newSlot.GetComponentInChildren<UnitDraggable>();

            if (unitDraggable != null)
            {
                unitDraggable.unitName = unitData.Key;
                unitDraggables.Add(unitDraggable);
            }
        }
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
                    
                    if (prefabname.name == "Default")
                    {
                        return;
                    }
                    unintDropable.RemoveExistingPrefab(slotIndex);
                    GameObject prefabObject = Instantiate(prefabname, Slot[slotIndex].transform);
                    RectTransform prefab = prefabObject.GetComponent<RectTransform>();
                    RectTransform slotRect = Slot[slotIndex].GetComponent<RectTransform>();

                    prefab.sizeDelta = slotRect.sizeDelta;
                    prefab.localScale = new Vector3(200, 200, 1);
                    prefab.anchoredPosition = new Vector2(0, -50);
                    
                    Slot[slotIndex].GetComponent<UnitDropable>().enabled = false;
                    
                    RemoveClones();
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
        CustomLogger.Log("리스트에서 리무브 하는 메서드 발동", Color.magenta);
        foreach (Transform child in slotParent)
        {
            TextMeshProUGUI unitText = child.GetComponentInChildren<TextMeshProUGUI>();

            if (unitText != null && unitText.text == unitName && child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(false);
                break;
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

        foreach (var unitDropable in Slot)
        {
            unitDropable.GetComponent<UnitDropable>().enabled = true;
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

    public void RemoveClones()
    {
        foreach (Transform child in panelTransform)
        {
            if (child.name.EndsWith("(Clone)"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void UpdateUnitsList()
    {
        Dictionary<string, int> deployedUnitCounts = new Dictionary<string, int>();

        foreach (var selectedUnit in selectedUnits)
        {
            string unitName = selectedUnit.Value.Trim();
            CustomLogger.Log("Checking deployed unit: " + unitName, Color.magenta);

            if (!deployedUnitCounts.TryAdd(unitName, 1))
            {
                deployedUnitCounts[unitName]++;
            }
        }

        foreach (Transform unitSlot in slotParent)
        {
            UnitDraggable unitDraggable = unitSlot.GetComponentInChildren<UnitDraggable>();

            if (unitDraggable != null)
            {
                string unitName = unitDraggable.unitName.Trim();
                if (deployedUnitCounts.ContainsKey(unitName) && deployedUnitCounts[unitName] > 0)
                {
                    unitSlot.gameObject.SetActive(false);
                    deployedUnitCounts[unitName]--;
                    if (deployedUnitCounts[unitName] <= 0)
                    {
                        deployedUnitCounts.Remove(unitName);
                    }
                }
            }
        }
    }
}
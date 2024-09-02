using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public List<Triple<int, int, string>> userUnits; // 유저 구매 유닛 및 가진 유닛들
    private List<UnitDraggable> unitDraggables = new List<UnitDraggable>();

    private void Awake()
    {
        LoadUserUnit();
        if (countPlacedUnit() > 0)
        {
            DisplayPrefab();
        }

        DisplayUnitsList();
        //UpdateUnitsList();
    }

    private int countPlacedUnit()
    {
        int ret = 0;
        foreach (var t in userUnits)
        {
            if (t.Item2 > -1)
            {
                ret++;
            }
        }

        return ret;
    }

    private void LoadUserUnit()
    {
        userUnits = PlayerLocalManager.Instance.lUnitList;
    }

    public void resetUnitsList()
    {
        ResetList();
        LoadNextScene();
    }
    
    public void LoadNextScene(){
        StartCoroutine(LoadMyAsyncScene());
    }

    IEnumerator LoadMyAsyncScene()
    {    
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    
    private void DisplayUnitsList()
    {
        Sprite[] prefabImages = Resources.LoadAll<Sprite>("Image/Unit");
        foreach (var unitData in userUnits)
        {
            if (unitData.Item3 != "Default" && unitData.Item2 < 0)
            {
                GameObject newSlot = Instantiate(slotPrefab, slotParent);
                Image unitImage = newSlot.GetComponentInChildren<Image>();

                if (unitImage != null)
                {
                    foreach (var prefabImage in prefabImages)
                    {
                        string spriteNameWithoutSuffix = prefabImage.name.Split('_')[0];
                        if (spriteNameWithoutSuffix == unitData.Item3)
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
                    unitDraggable.unitName = unitData.Item3;
                    unitDraggable.unitIndex = unitData.Item1;
                    unitDraggables.Add(unitDraggable);
                }
            }
        }
    }

    public void DisplayPrefab()
    {
        Prefabs = Resources.LoadAll<GameObject>("Defense/Unit");
        if (countPlacedUnit() > 0)
        {
            foreach (var unitData in userUnits)
            {
                int slotIndex = unitData.Item2;
                string unitName = unitData.Item3;

                GameObject prefabname = FindPrefabByName(unitName);

                if (prefabname != null && prefabname.name != "Default" && slotIndex > -1)
                {
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
            Image unitText = child.GetComponent<Image>();
            string unit = unitText.sprite.name.Split('_')[0];
            if (unitText != null && unit == unitName && child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(false);
                break;
            }
        }
    }

    public void ResetList()
    {
        foreach (var t in userUnits)
        {
            t.Item2 = -1;
        }

        PlayerLocalManager.Instance.lUnitList = userUnits;
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

        foreach (var selectedUnit in userUnits)
        {
            string unitName = selectedUnit.Item3.Trim();
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
                        CustomLogger.Log("Remove deployed unit: " + unitName, Color.red);
                    }
                }
            }
        }
    }
}
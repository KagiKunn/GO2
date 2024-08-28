using System;
using System.Collections.Generic;
using Org.BouncyCastle.Cms;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

[System.Serializable]
public class UnitList
{
    [SerializeField] private string className;
    [SerializeField] private GameObject[] unitList;

    // className의 속성
    public string ClassName
    {
        get { return className; }
    }

    // unitList의 속성
    public GameObject[] UnitListArray
    {
        get { return unitList; }
    }
}

public class UnitUpgrade : MonoBehaviour
{
    [ArrayElementTitle("className")] [SerializeField]
    public UnitList[] upgradeList;

    public GameObject leftBox, rightBox, origin, upgrade, prevMage, nextMage;
    public int[] levelList = new int[4];
    public int classNum = 0;
    public int mageClass = 0;
    public GameObject[] Prefabs; // 리소스 폴더 내 유닛 프리팹
    public GameObject UnitListView;
    public LayerMask clickableLayer;
    public string selected;
    private Button btn;
    [SerializeField] private TMP_Text priceTxt;
    private int price;
    private GameObject upConfirm, upOk;

    void Start()
    {
        clickableLayer = LayerMask.NameToLayer("Ally");
    }

    void Awake()
    {
        leftBox = GameObject.Find("UnitFormation");
        rightBox = GameObject.Find("UnitFormation(Reinforce)");
        prevMage = GameObject.Find("PrevMage");
        nextMage = GameObject.Find("NextMage");
        upConfirm = GameObject.Find("UpConfirm");
        upOk = GameObject.Find("UpOk");
        btn = GameObject.Find("UpgradeBtn").GetComponent<Button>();

        upConfirm.SetActive(false);
        upOk.SetActive(false);
        /*origin = Instantiate(upgradeList[classNum].UnitListArray[levelList[0]]);
        upgrade = Instantiate(upgradeList[classNum].UnitListArray[levelList[0] + 1]);
        origin.transform.localScale = new Vector3(2.5f, 2.5f);
        upgrade.transform.localScale = new Vector3(2.5f, 2.5f);
        origin.transform.position = leftBox.transform.position;
        upgrade.transform.position = rightBox.transform.position;*/
        DisplayPrefab();
    }

    private void Update()
    {
        if (classNum != 3)
        {
            prevMage.SetActive(false);
            nextMage.SetActive(false);
        }
        else
        {
            if (levelList[3] == 1)
            {
                prevMage.SetActive(true);
                nextMage.SetActive(true);
            }
            else
            {
                prevMage.SetActive(false);
                nextMage.SetActive(false);
            }
        }
        bool isInteractable = !(string.IsNullOrEmpty(selected) || selected.Split("_")[1].Split(":")[0].Split("-")[1] == "2");

        if (PlayerLocalManager.Instance.lMoney < price)
        {
            isInteractable = false;
        }

        btn.interactable = isInteractable;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (upConfirm.active)
            {
                upConfirm.SetActive(false);
            }
            else
            {
                SceneManager.LoadScene("InternalAffairs");
            }
        }

        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
            Collider2D collider = Physics2D.OverlapPoint(mousePosition2D); // 특정 레이어만 감지
            if (collider != null)
            {
                GameObject clickedObject = collider.gameObject;
                int unitIndex = clickedObject.GetComponent<UnitIndex>().unitIndex;
                classNum = FindPrefabClassNum(clickedObject.transform.parent.name.Split("_")[1]);
                levelList[classNum] = FindPrefabClassUpNum(clickedObject.transform.parent.name.Split("_")[1]);
                selected =
                    $"{unitIndex}_{classNum}-{levelList[classNum]}:{clickedObject.transform.parent.name.Split("_")[1]}";
                updateHero();
            }
        }
    }

    void updateHero()
    {
        switch (levelList[classNum])
        {
            case 0:
                price = 80;
                priceTxt.text = $"{price}G/{PlayerLocalManager.Instance.lMoney}G";
                break;
            case 1:
                price = 100;
                priceTxt.text = $"{price}G/{PlayerLocalManager.Instance.lMoney}G";
                break;
            default:
                price = 0;
                priceTxt.text = "MAX";
                break;
        }
        Destroy(origin);
        Destroy(upgrade);
        origin = Instantiate(upgradeList[classNum].UnitListArray[levelList[classNum]]);
        origin.transform.localScale = new Vector3(2.5f, 2.5f);
        origin.transform.position = leftBox.transform.position;
        if (classNum != 3)
        {
            if (levelList[classNum] < upgradeList[classNum].UnitListArray.Length - 1)
            {
                upgrade = Instantiate(upgradeList[classNum].UnitListArray[levelList[classNum] + 1]);
                upgrade.transform.localScale = new Vector3(2.5f, 2.5f);
                upgrade.transform.position = rightBox.transform.position;
            }
        }
        else
        {
            if (levelList[classNum] < 2)
            {
                upgrade = Instantiate(upgradeList[classNum].UnitListArray[levelList[classNum] + 1 + mageClass]);
                upgrade.transform.localScale = new Vector3(2.5f, 2.5f);
                upgrade.transform.position = rightBox.transform.position;
            }
        }
    }

    public void Upgrade()
    {
        if (classNum == 3)
        {
            if (levelList[classNum] == 0)
            {
                UpgradeUnit(1);
            }
            else if (levelList[classNum] == 1)
            {
                priceTxt.text = $"100G/{PlayerLocalManager.Instance.lMoney}";
                UpgradeUnit(1 + mageClass);
            }
        }
        else
        {
            if (levelList[classNum] >= upgradeList[classNum].UnitListArray.Length - 1) return;
            UpgradeUnit(1);
        }
    }

    private void UpgradeUnit(int increment)
    {
        //0 Bow, 1 Crossbow, 2 Gun, 3 Mage
        //Mage Class Value 0 = fire, 1 = ice, 2 = lightning
        levelList[classNum] += increment;
        string newUnitName = upgradeList[classNum].UnitListArray[levelList[classNum]].name;
        int selectedIdx = Convert.ToInt32(selected.Split("_")[0]);
        var newKeyValuePair = new KeyValuePair<string, int>(newUnitName, selectedIdx);
        PlayerLocalManager.Instance.lUnitList[selectedIdx] = newKeyValuePair;
        PlayerLocalManager.Instance.lMoney -= price;
        PlayerLocalManager.Instance.Save();
        Transform content = UnitListView.transform.Find("Viewport/Content");
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        DisplayPrefab();
        updateHero();
        upConfirm.SetActive(false);
    }

    public void openConfirm()
    {
        upConfirm.SetActive(true);
    }

    public void changeClass(int val)
    {
        var maxIndex = upgradeList.Length - 1;
        classNum = (classNum + val + maxIndex + 1) % (maxIndex + 1);
        updateHero();
    }

    public void changeMageClass(int val)
    {
        var maxIndex = 2;
        mageClass = (mageClass + val + maxIndex + 1) % (maxIndex + 1);
        updateHero();
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
        return null;
    }

    public int FindPrefabClassNum(string unitName)
    {
        int classNum = -1;
        //0 Bow, 1 Crossbow, 2 Gun, 3 Mage
        if (unitName.Contains("Cross"))
        {
            classNum = 1;
        }
        else if (unitName.Contains("Bow"))
        {
            classNum = 0;
        }
        else if (unitName.Contains("Gun"))
        {
            classNum = 2;
        }
        else if (unitName.Contains("Mage"))
        {
            classNum = 3;
        }

        return classNum;
    }

    public int FindPrefabClassUpNum(string unitName)
    {
        int classNum = -1;
        if (unitName.Contains("Hero") || unitName.Contains("Fire") || unitName.Contains("Ice") ||
            unitName.Contains("Lighting"))
        {
            classNum = 2;
        }
        else if (unitName.Contains("Master") || unitName.Contains("Arc"))
        {
            classNum = 1;
        }
        else
        {
            mageClass = 0;
            classNum = 0;
        }

        return classNum;
    }

    public void DisplayPrefab()
    {
        Prefabs = Resources.LoadAll<GameObject>("Defense/Unit");
        float xOffset = 0f;
        if (PlayerLocalManager.Instance.lUnitList != null)
        {
            float spacing = 10f;
            int index = 0;
            RectTransform contentRect = UnitListView.transform.Find("Viewport/Content").GetComponent<RectTransform>();

            var unitList = PlayerLocalManager.Instance.lUnitList;

            for (int i = 0; i < unitList.Count; i++)
            {
                var unitData = unitList[i];
                string unitName = unitData.Key;
                int slotIndex = i;

                GameObject prefabname = FindPrefabByName(unitName);

                if (prefabname != null && !unitName.Equals("Default"))
                {
                    index++;
                    GameObject prefabObject = Instantiate(prefabname, contentRect);
                    prefabObject.name = $"{slotIndex}_{unitName}";
                    prefabObject.transform.GetChild(0).gameObject.AddComponent<UnitIndex>().unitIndex = i;
                    prefabObject.transform.GetChild(0).gameObject.AddComponent<UnitIndex>().unitName = unitName;
                    RectTransform prefab = prefabObject.GetComponent<RectTransform>();
                    prefab.localScale = new Vector3(200, 200, 1);

                    Image image = prefabObject.AddComponent<Image>();
                    image.color = new Color(1, 1, 1, 0);

                    prefab.anchoredPosition = new Vector2(xOffset, 0);
                    xOffset = index * 200;
                }
            }

            contentRect.sizeDelta = new Vector2(xOffset / 2 - 600, contentRect.sizeDelta.y);
        }
    }
}
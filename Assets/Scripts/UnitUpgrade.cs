using System.Collections.Generic;
using Org.BouncyCastle.Cms;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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

    //0 Bow, 1 Crossbow, 2 Gun, 3 Mage
    public int mageClass = 0;
    public GameObject[] Prefabs; // 리소스 폴더 내 유닛 프리팹

    public GameObject UnitListView;
    //Mage Class Value 0 = fire, 1 = ice, 2 = lightning
    public LayerMask clickableLayer;
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
        origin = Instantiate(upgradeList[classNum].UnitListArray[levelList[0]]);
        upgrade = Instantiate(upgradeList[classNum].UnitListArray[levelList[0] + 1]);
        origin.transform.localScale = new Vector3(2.5f, 2.5f);
        upgrade.transform.localScale = new Vector3(2.5f, 2.5f);
        origin.transform.position = leftBox.transform.position;
        upgrade.transform.position = rightBox.transform.position;
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("InternalAffairs");
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
                CustomLogger.Log(clickedObject.transform.parent.name+","+unitIndex);
            }

            // Vector2 localMousePosition = UnitListView.transform.Find("Viewport/Content").GetComponent<RectTransform>().InverseTransformPoint(Input.mousePosition);
            //
            // foreach (Transform child in UnitListView.transform.Find("Viewport/Content"))
            // {
            //     RectTransform childRect = child.GetComponent<RectTransform>();
            //     if (RectTransformUtility.RectangleContainsScreenPoint(childRect, Input.mousePosition, null))
            //     {
            //         // 클릭된 오브젝트의 정보를 처리합니다.
            //         ShowPrefabInfo(child.gameObject);
            //         break; // 첫 번째로 감지된 오브젝트만 처리한다면 반복문을 종료합니다.
            //     }
            // }
        }
    }

    void updateHero()
    {
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
                levelList[classNum] += 1;
                updateHero();
            }
            else if (levelList[classNum] == 1)
            {
                levelList[classNum] += 1 + mageClass;
                updateHero();
            }
        }
        else
        {
            if (levelList[classNum] >= upgradeList[classNum].UnitListArray.Length - 1) return;
            levelList[classNum] += 1;
            updateHero();
        }
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

    void debugOutput()
    {
        for (int i = 0; i < upgradeList.Length; i++)
        {
            // className 출력
            string className = upgradeList[i].ClassName;
            CustomLogger.LogWarning("Class Name: " + className);

            // unitList 출력
            GameObject[] units = upgradeList[i].UnitListArray;
            if (className != "Mage")
            {
                for (int j = 0; j < units.Length; j++)
                {
                    if (j < units.Length - 1)
                    {
                        CustomLogger.LogWarning(units[j].name + " > " + units[j + 1].name);
                    }
                    else
                    {
                        CustomLogger.LogWarning(units[j].name);
                    }
                }
            }
            else
            {
                CustomLogger.LogWarning(units[0].name + " > " + units[1].name);
                for (int j = 2; j < units.Length; j++)
                {
                    CustomLogger.LogWarning(units[1].name + " > " + units[j].name);
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

    public void DisplayPrefab()
    {
        Prefabs = Resources.LoadAll<GameObject>("Defense/Unit");
        float xOffset = 0f; // 처음 X 위치는 0으로 시작
        if (PlayerLocalManager.Instance.lUnitList != null)
        {
            float spacing = 10f; // 각 프리팹 사이의 간격
            int index = 0;
            // Content 오브젝트의 RectTransform을 가져옵니다.
            RectTransform contentRect = UnitListView.transform.Find("Viewport/Content").GetComponent<RectTransform>();

            var unitList = PlayerLocalManager.Instance.lUnitList;

            for (int i = 0; i < unitList.Count; i++)
            {
                var unitData = unitList[i];
                string unitName = unitData.Key;
                int slotIndex = i; // i가 곧 인덱스

                GameObject prefabname = FindPrefabByName(unitName);

                if (prefabname != null && !unitName.Equals("Default"))
                {
                    index++;
                    GameObject prefabObject = Instantiate(prefabname, contentRect);
                    prefabObject.transform.GetChild(0).gameObject.AddComponent<UnitIndex>().unitIndex = i;
                    RectTransform prefab = prefabObject.GetComponent<RectTransform>();
                    prefab.localScale = new Vector3(200, 200, 1);

                    Image image = prefabObject.AddComponent<Image>();
                    image.color = new Color(1, 1, 1, 0);

                    Button button = prefabObject.AddComponent<Button>();
        
                    // 캡처된 변수들을 명확히 전달
                    int capturedSlotIndex = slotIndex;
                    string capturedUnitName = unitName;

                    prefab.anchoredPosition = new Vector2(xOffset, 0);
                    CustomLogger.LogWarning($"slotIndex:{capturedSlotIndex} | unitName:{capturedUnitName} | PrefabName:{prefabObject.name}");
                    xOffset = index * 200;
                }
            }

            contentRect.sizeDelta = new Vector2(xOffset / 2 - 600, contentRect.sizeDelta.y);
        }
    }



    void ShowPrefabInfo(GameObject clickedObject)
    {
        // 여기서 필요한 정보를 가져와 출력합니다.
        string unitName = clickedObject.name; // 예를 들어, 이름을 가져온다.
        CustomLogger.LogWarning($"Clicked on Prefab: {unitName}");
        CustomLogger.LogWarning($"Pos: {clickedObject.transform.position}");

        // 추가로 필요한 로직을 여기에 구현할 수 있습니다.
    }
}
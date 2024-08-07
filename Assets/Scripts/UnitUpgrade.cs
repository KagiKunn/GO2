using Unity.VisualScripting;
using UnityEngine;


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


    public GameObject leftBox, rightBox, origin, upgrade;
    public int[] levelList = new int[4];
    public int classNum = 0;

    void Start()
    {
    }

    void Awake()
    {
        leftBox = GameObject.Find("UnitFormation");
        rightBox = GameObject.Find("UnitFormation(Reinforce)");
        origin = Instantiate(upgradeList[classNum].UnitListArray[levelList[0]]);
        upgrade = Instantiate(upgradeList[classNum].UnitListArray[levelList[0] + 1]);
        origin.transform.localScale = new Vector3(2.5f, 2.5f);
        upgrade.transform.localScale = new Vector3(2.5f, 2.5f);
        origin.transform.position = leftBox.transform.position;
        upgrade.transform.position = rightBox.transform.position;
    }

    public void Upgrade()
    {
        if (classNum == 3)
        {
        }
        else
        {
            if (levelList[classNum] >= upgradeList[classNum].UnitListArray.Length - 1) return;
            Destroy(origin);
            Destroy(upgrade);
            levelList[classNum] += 1;
            origin = Instantiate(upgradeList[classNum].UnitListArray[levelList[classNum]]);
            origin.transform.localScale = new Vector3(2.5f, 2.5f);
            origin.transform.position = leftBox.transform.position;
            if (levelList[classNum] < upgradeList[classNum].UnitListArray.Length - 1)
            {
                upgrade = Instantiate(upgradeList[classNum].UnitListArray[levelList[0] + 1]);
                upgrade.transform.localScale = new Vector3(2.5f, 2.5f);
                upgrade.transform.position = rightBox.transform.position;
            }
        }
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
}
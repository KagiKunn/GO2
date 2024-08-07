using System;
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


    public GameObject leftBox, rightBox, origin, upgrade, prevMage, nextMage;
    public int[] levelList = new int[4];
    public int classNum = 0;
    //0 Bow, 1 Crossbow, 2 Gun, 3 Mage
    public int mageClass = 0;
    //Mage Class Value 0 = fire, 1 = ice, 2 = lightning

    void Start()
    {
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
}
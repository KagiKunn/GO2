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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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

    // Update is called once per frame
    void Update()
    {
    }
}
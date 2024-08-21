using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UnitShopInit : MonoBehaviour
{
    public GameObject[] units;
    private int unitIndex = 0;

    private List<string> boughtUnit = new List<string>();
    private void Awake()
    {
        GameObject prefabObject = Instantiate(units[unitIndex],transform.position,quaternion.identity,transform);
        RectTransform newObjectRect = prefabObject.GetComponent<RectTransform>();

        newObjectRect.localScale = new Vector3(200, 200, 1);
        newObjectRect.anchoredPosition = new Vector2(0,-50);
    }

    public void NextUnitButton()
    {
        unitIndex++;
        if (unitIndex > units.Length-1)
        {
            unitIndex = 0;
        }

        OtherUnit();
    }
    public void PrevUnitButton()
    {
        unitIndex--;
        if (unitIndex < 0)
        {
            unitIndex = units.Length-1;
        }

        OtherUnit();
    }

    private void OtherUnit()
    {
        Destroy(transform.GetChild(0).gameObject);
        GameObject prefabObject = Instantiate(units[unitIndex],transform.position,quaternion.identity,transform);
        RectTransform newObjectRect = prefabObject.GetComponent<RectTransform>();

        newObjectRect.localScale = new Vector3(200, 200, 1);
        newObjectRect.anchoredPosition = new Vector2(0,-50);
    }

    public void BuyUnit()
    {
        int price = unitIndex switch
        {
            0 => 100,
            1 => 100,
            2 => 150,
            3 => 200,
            _ => 0
        };
        if (PlayerLocalManager.Instance.lMoney < price)
        {
            CustomLogger.Log("Need More Money!","red");
            return;
        }
        
        
        string unit = unitIndex switch
        {
            0 => "Bow",
            1 => "Crossbow",
            2 => "Gun",
            3 => "Mage",
            _ => "err"
        };
        boughtUnit.Add(unit);
        PlayerLocalManager.Instance.lMoney -= price;
        //여기서 유닛 세이브
        
        
        foreach (string b in boughtUnit)
        {
            CustomLogger.Log(b);
        }
        PlayerLocalManager.Instance.Save();
    }
}

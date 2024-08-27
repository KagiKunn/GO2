using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;

public class UnitShopInit : MonoBehaviour
{
    public GameObject[] units;
    public SimplePopup popup;
    public LocalizedString saveString;
    
    private int unitIndex = 0;

    private List<KeyValuePair<string, int>> boughtUnit;
    private void Awake()
    {
        boughtUnit = PlayerLocalManager.Instance.lUnitList ?? new List<KeyValuePair<string, int>>();
        GameObject prefabObject = Instantiate(units[unitIndex],transform.position,quaternion.identity,transform);
        RectTransform newObjectRect = prefabObject.GetComponent<RectTransform>();

        newObjectRect.localScale = new Vector3(200, 200, 1);
        newObjectRect.anchoredPosition = new Vector2(0,-50);
        GameObject.Find("RealPrice").GetComponent<TextMeshProUGUI>().text = UnitPrice().ToString();
        GameObject.Find("Name").GetComponent<TextMeshProUGUI>().text = UnitName();
    }

    private void Start()
    {
        saveString.TableReference = "UI";
        saveString.TableEntryReference = "Heired Success";
    }

    public void Back()
    {
        SceneManager.LoadScene("InternalAffairs");
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
        GameObject.Find("Name").GetComponent<TextMeshProUGUI>().text = UnitName();
        GameObject.Find("RealPrice").GetComponent<TextMeshProUGUI>().text = UnitPrice().ToString();
    }

    private int UnitPrice()
    {
        int price = unitIndex switch
        {
            0 => 100,
            1 => 100,
            2 => 150,
            3 => 200,
            _ => 0
        };
        return price;
    }

    private string UnitName()
    {
        string unit = unitIndex switch
        {
            0 => "Bow",
            1 => "Crossbow",
            2 => "Gun",
            3 => "Mage",
            _ => "err"
        };
        return unit;
    }
    public void BuyUnit()
    {
        int price = UnitPrice();
        if (PlayerLocalManager.Instance.lMoney < price)
        {
            CustomLogger.Log("Need More Money!", "red");
            return;
        }

        string unit = UnitName();
        KeyValuePair<string, int> keyVal = new KeyValuePair<string, int>(unit, 0);
        boughtUnit.Add(keyVal);
        PlayerLocalManager.Instance.lMoney -= price;

        // 유닛 세이브
        PlayerLocalManager.Instance.lUnitList = boughtUnit;
        PlayerLocalManager.Instance.Save();

        // 간단한 팝업 호출
        popup.ShowPopup("Unit Purchased: " + unit);
    }
}

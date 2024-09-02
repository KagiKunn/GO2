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

    private List<Triple<int, int, string>> boughtUnit;

    private void Awake()
    {
        boughtUnit = PlayerLocalManager.Instance.lUnitList ?? new List<Triple<int, int, string>>();
        GameObject prefabObject = Instantiate(units[unitIndex], transform.position, quaternion.identity, transform);
        RectTransform newObjectRect = prefabObject.GetComponent<RectTransform>();

        newObjectRect.localScale = new Vector3(200, 200, 1);
        newObjectRect.anchoredPosition = new Vector2(0, -50);
        GameObject.Find("RealPrice").GetComponent<TextMeshProUGUI>().text = $"{UnitPrice().ToString()}G";
        LocalizedString localizedString = new LocalizedString
            { TableReference = "UI", TableEntryReference = $"{UnitName()}" };
        localizedString.StringChanged += (localizedText) =>
        {
            GameObject.Find("Name").GetComponent<TextMeshProUGUI>().text = $"{localizedText}";
        };
        localizedString = new LocalizedString
            { TableReference = "UI", TableEntryReference = $"{UnitName()}Desc" };
        localizedString.StringChanged += (localizedText) =>
        {
            GameObject.Find("Description").GetComponent<TextMeshProUGUI>().text = $"{localizedText}";
        };
    }

    private void Start()
    {
        saveString.TableReference = "UI";
        saveString.TableEntryReference = "Heired Success";
    }

    private string Info()
    {
        string info = unitIndex switch
        {
            0 => "궁병입니다. 준수한 공격속도와 사거리 그리고 낮은 가격의 기본유닛입니다. 업그레이드시 단일적에게 높은 피해를 주는 대궁을 사용합니다.",
            1 => "석궁병입니다. 사거리는 짧지만 공격속도가 높은 기본유닛입니다. 업그레이드시 공격속도가 매우 빨라집니다.",
            2 => "포병입니다. 공격속도는 느리지만 데미지가 강하며 공격즉시 피해를 입힙니다. 업그레이드시 적을 기절을 시키는 범위공격을 합니다.",
            3 => "마법사입니다 범위공격을 하며 가장 비싼유닛입니다. 업그레이드시 속성을 부여할 수 있습니다. 불:지속데미지, 얼음:느려짐, 번개:받는피해증가",
            _ => "잘못된 데이터"
        };
        return info;
    }

    public void Back()
    {
        SceneManager.LoadScene("InternalAffairs");
    }

    public void NextUnitButton()
    {
        unitIndex++;
        if (unitIndex > units.Length - 1)
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
            unitIndex = units.Length - 1;
        }

        OtherUnit();
    }

    private void OtherUnit()
    {
        Destroy(transform.GetChild(0).gameObject);
        GameObject prefabObject = Instantiate(units[unitIndex], transform.position, quaternion.identity, transform);
        RectTransform newObjectRect = prefabObject.GetComponent<RectTransform>();

        newObjectRect.localScale = new Vector3(200, 200, 1);
        newObjectRect.anchoredPosition = new Vector2(0, -50);
        GameObject.Find("RealPrice").GetComponent<TextMeshProUGUI>().text = $"{UnitPrice().ToString()}G";

        LocalizedString localizedString = new LocalizedString
            { TableReference = "UI", TableEntryReference = $"{UnitName()}" };
        localizedString.StringChanged += (localizedText) =>
        {
            GameObject.Find("Name").GetComponent<TextMeshProUGUI>().text = $"{localizedText}";
        };

        localizedString = new LocalizedString
            { TableReference = "UI", TableEntryReference = $"{UnitName()}Desc" };
        localizedString.StringChanged += (localizedText) =>
        {
            GameObject.Find("Description").GetComponent<TextMeshProUGUI>().text = $"{localizedText}";
        };
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
            0 => "Bowman",
            1 => "Crossbowman",
            2 => "Artillery",
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
            popup.ShowPopup("Need More Money!");
            CustomLogger.Log("Need More Money!", "red");
            return;
        }

        string unit = UnitName();
        
        LocalizedString localizedString = new LocalizedString
            { TableReference = "UI", TableEntryReference = $"{UnitName()}" };
        
        localizedString.StringChanged += (localizedText) =>
        {
            popup.ShowPopup($"Unit Purchased: {localizedText}");
        };

        if (unit.Equals("Bowman"))
        {
            unit = "Bow";
        }else if (unit.Equals("Crossbowman"))
        {
            unit = "Crossbow";
        }else if (unit.Equals("Artillery"))
        {
            unit = "Gun";
        }

        var nextIndex = Triple<int,int,string>.GetTripleWithMaxT1<int, int, string>(PlayerLocalManager.Instance.lUnitList);
        Triple<int, int, string> keyVal = new Triple<int, int, string>(nextIndex.Item1+1,-1,unit);
        boughtUnit.Add(keyVal);
        PlayerLocalManager.Instance.lMoney -= price;

        // 유닛 세이브
        PlayerLocalManager.Instance.lUnitList = boughtUnit;
        PlayerLocalManager.Instance.Save();
    }
}
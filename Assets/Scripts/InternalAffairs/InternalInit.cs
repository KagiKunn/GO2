using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class InternalInit : MonoBehaviour
{
    private GameObject current;
    private GameObject max;

    private GameObject gold;
    private GameObject soul;
    void Start()
    {
        current = GameObject.Find("Current");
        max = GameObject.Find("Max");
        gold = GameObject.Find("GoldInput");
        soul = GameObject.Find("SoulInput");
        
        current.GetComponent<TextMeshProUGUI>().text = PlayerLocalManager.Instance.lCastleHp.ToString(CultureInfo.CurrentCulture);
        max.GetComponent<TextMeshProUGUI>().text = PlayerLocalManager.Instance.lCastleMaxHp.ToString(CultureInfo.CurrentCulture);
        gold.GetComponent<TMP_InputField>().text = PlayerLocalManager.Instance.lMoney.ToString();
        soul.GetComponent<TMP_InputField>().text = PlayerLocalManager.Instance.lPoint.ToString();
    }

    private void Update()
    {
        current.GetComponent<TextMeshProUGUI>().text = PlayerLocalManager.Instance.lCastleHp.ToString(CultureInfo.CurrentCulture);
        gold.GetComponent<TMP_InputField>().text = PlayerLocalManager.Instance.lMoney.ToString();
    }
}

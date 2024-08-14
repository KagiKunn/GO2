using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResetPointLevel : MonoBehaviour
{
    public Button toggleButton; // 활성화/비활성화 버튼
    private int level = 0;
    private Image buttonImage;
    private SoulCalc soulCalc;
    void Start()
    {

        if (toggleButton == null)
        {
            toggleButton = GetComponentInChildren<Button>();
        }
        // 버튼에 클릭 이벤트 연결
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ResetObject);
        }
        else
        {
            Debug.LogError("Toggle Button is not assigned and could not be found in children!");
        }
    }
    void ResetObject()
    {
        level = gameObject.transform.parent.GetComponent<UIButtonToggle>().level;
        Transform levelBar = gameObject.transform.parent.Find("LevelBar");
        for (int i = 0; i < 4; i++)
        {
            levelBar.GetChild(i).GetComponent<SpriteRenderer>().color=Color.white;
        }
        buttonImage = gameObject.transform.parent.GetComponent<Image>();
        soulCalc = GameObject.Find("Soul").transform.GetChild(0).GetComponent<SoulCalc>();
        if (level > 0)
        {
            gameObject.transform.parent.GetComponent<UIButtonToggle>().level = 0;
            for (int i = 0; i < level; i++)
            {
                soulCalc.SoulIncDec(false);
            }
            buttonImage.color=Color.white;
        }
    }
}
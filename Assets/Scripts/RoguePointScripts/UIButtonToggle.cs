using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonToggle : MonoBehaviour
{
    public Button toggleButton; // 활성화/비활성화 버튼
    public int level = 0;
    
    private Image buttonImage;
    private SoulCalc soulCalc;
    void Start()
    {
        soulCalc = GameObject.Find("Soul").transform.GetChild(0).GetComponent<SoulCalc>();
        if (toggleButton == null)
        {
            toggleButton = GetComponentInChildren<Button>();
        }

        // 버튼에 클릭 이벤트 연결
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleObject);

            // Image 컴포넌트 가져오기
            buttonImage = gameObject.GetComponent<Image>();
            if (buttonImage != null)
            {
                if (level>0)
                {
                    buttonImage.color=Color.black;
                }
                else
                {
                    buttonImage.color=Color.white;
                }
                
                Debug.Log("Image component found on the GameObject.");
            }
            else
            {
                Debug.LogError("No Image component found on the GameObject.");
            }
        }
        else
        {
            Debug.LogError("Toggle Button is not assigned and could not be found in children!");
        }
    }


    void ToggleObject()
    {
        // targetObject의 활성화 상태를 전환
        UpdateButton(); // 버튼 텍스트 업데이트
    }

    void UpdateButton()
    {
        // 현재 상태에 따라 버튼의 텍스트를 변경
        if (level is < 4)
        {
            if (soulCalc.SoulIncDec(true))
            {
                buttonImage.color = Color.black;
                level++;
                CustomLogger.Log(level);
            }
        }
        else
        {
            CustomLogger.Log("rage must be 0~4");
        } 
    }
}
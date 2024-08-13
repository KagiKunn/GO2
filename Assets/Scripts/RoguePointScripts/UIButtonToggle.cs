using UnityEngine;
using UnityEngine.UI;

public class UIButtonToggle : MonoBehaviour
{
    public Button toggleButton; // 활성화/비활성화 버튼
    public bool isActive; // 오브젝트의 현재 상태를 추적

    private Image buttonImage;
    void Start()
    {
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
                if (isActive)
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

            UpdateButton();
        }
        else
        {
            Debug.LogError("Toggle Button is not assigned and could not be found in children!");
        }
    }


    void ToggleObject()
    {
        // targetObject의 활성화 상태를 전환
        isActive = !isActive;
        UpdateButton(); // 버튼 텍스트 업데이트
    }

    void UpdateButton()
    {
        // 현재 상태에 따라 버튼의 텍스트를 변경
        if (isActive)
        {
            buttonImage.color = Color.black;
            CustomLogger.Log("Active!");
        }
        else
        {
            buttonImage.color = Color.white;
            CustomLogger.Log("DeActive!");
        }
    }
}
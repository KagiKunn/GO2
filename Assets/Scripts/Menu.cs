using UnityEngine;
using UnityEngine.UIElements;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private UIDocument uiDocument;
    private Button menuButton;
    private Button speedButton;
    public GameObject settingObject;
    private void Awake()
    {
        menuButton = uiDocument.rootVisualElement.Q<Button>("MenuButton");
        speedButton = uiDocument.rootVisualElement.Q<Button>("SpeedButton");
        if (menuButton != null && speedButton != null)
        {
            menuButton.clicked += OnMenuButtonClicked;
            speedButton.clicked += OnSpeedButtonClicked;
        }
    }
    private void OnMenuButtonClicked()
    {
        // 버튼 클릭 시 실행할 코드를 여기에 작성합니다.
        CustomLogger.Log("MenuButton clicked!","yellow");
        if (Time.timeScale <= 0)
        {
            Time.timeScale = 1;
            settingObject.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            settingObject.SetActive(true);
        }
    }

    private void OnSpeedButtonClicked()
    {
        CustomLogger.Log("SpeedButton clicked!","yellow");
        if (Time.timeScale < 2)
        {
            Time.timeScale = 2;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private UIDocument uiDocument;
    private Button menuButton;
    private Button speedButton;
    private Button giveupButton;
    public GameObject settingObject;
    private void Awake()
    {
        menuButton = uiDocument.rootVisualElement.Q<Button>("MenuButton");
        speedButton = uiDocument.rootVisualElement.Q<Button>("SpeedButton");
        giveupButton = uiDocument.rootVisualElement.Q<Button>("GiveupButton");
        if (menuButton != null && speedButton != null && giveupButton !=null)
        {
            menuButton.clicked += OnMenuButtonClicked;
            speedButton.clicked += OnSpeedButtonClicked;
            giveupButton.clicked += OnGiveupButtonClicked;
        }
        else
        {
            CustomLogger.Log("something is null");
            CustomLogger.Log(menuButton);
            CustomLogger.Log(speedButton);
            CustomLogger.Log(giveupButton);
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

    private void OnGiveupButtonClicked()
    {
        CustomLogger.Log("Giveup!");
        SceneManager.LoadScene("Title");
    }
}

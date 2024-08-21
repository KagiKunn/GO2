using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Menu : MonoBehaviour
{
    private Button confirmButton;
    private Button cancelButton;
    private VisualElement exitPopUp;

    [SerializeField]
    private UIDocument uiDocument;
    private Button menuButton;
    private Button speedButton;
    private Button giveupButton;
    public GameObject settingObject;

    private void Awake()
    {
        var root = uiDocument.rootVisualElement;
        
        menuButton = root.Q<Button>("MenuButton");
        speedButton = root.Q<Button>("SpeedButton");
        giveupButton = root.Q<Button>("GiveupButton");
        
        // ExitPopUp 참조하기
        exitPopUp = root.Q("ExitPopUp");
        exitPopUp.style.display = DisplayStyle.None;
        
        // ExitPopUp 하위 버튼 참조하기
        confirmButton = exitPopUp.Q<Button>("ConfirmButton");
        cancelButton = exitPopUp.Q<Button>("CancelButton");
        
        confirmButton.clicked += OnConfirmButtonClicked;
        cancelButton.clicked += OnCancelButtonClicked;
        
        if (menuButton != null && speedButton != null && giveupButton != null)
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
        CustomLogger.Log("MenuButton clicked!", "yellow");
        if (Time.timeScale <= 0)
        {
            Time.timeScale = 1;
            StageC.Instance.isGamePaused = false;
            SettingManager.Instance.SetMainCamera();
            SettingManager.Instance.gameObject.SetActive(false);
            SetButtonInteractable(speedButton, true);
            SetButtonInteractable(giveupButton, true);
        }
        else
        {
            Time.timeScale = 0;
            StageC.Instance.isGamePaused = true;
            SettingManager.Instance.SetMainCamera();
            SettingManager.Instance.gameObject.SetActive(true);
            SettingManager.Instance.LoadText();
            SetButtonInteractable(speedButton, false);
            SetButtonInteractable(giveupButton, false);
            
        }
    }

    private void OnSpeedButtonClicked()
    {
        CustomLogger.Log("SpeedButton clicked!", "yellow");
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
        Time.timeScale = 0;
        StageC.Instance.isGamePaused = true;
        // ExitPopUp을 보이게 하기
        if (exitPopUp != null)
        {
            exitPopUp.style.display = DisplayStyle.Flex;
        }
        //나머지 버튼들 클릭 방지해서 일시정지 풀리는거 방지
        SetButtonInteractable(menuButton, false);
        SetButtonInteractable(speedButton, false);
    }
    
    private void OnConfirmButtonClicked()
    {
        SceneManager.LoadScene("Title");
    }
    
    private void OnCancelButtonClicked()
    {
        // ExitPopUp을 숨기기
        if (exitPopUp != null)
        {
            exitPopUp.style.display = DisplayStyle.None;
        }
        Time.timeScale = 1;
        StageC.Instance.isGamePaused = false;
        
        //종료 캔슬 시 다시 다른 버튼 활성화
        SetButtonInteractable(menuButton, true);
        SetButtonInteractable(speedButton, true);
    }
    
    // 버튼 상호작용 비활성화 메서드 추가
    public void DisableButtonInteractions()
    {
        SetButtonInteractable(menuButton, false);
        SetButtonInteractable(speedButton, false);
        SetButtonInteractable(giveupButton, false);
    }
    
    
    private void SetButtonInteractable(Button button, bool interactable)
    {
        if (button != null)
        {
            button.SetEnabled(interactable);
        }
    }
    
    
}


using UnityEngine;
using UnityEngine.UIElements;

public class Menu : MonoBehaviour
{
    private Button _menuButton;
    [SerializeField]
    private UIDocument uiDocument;
    private void Awake()
    {
        _menuButton = uiDocument.rootVisualElement.Q<Button>("MenuButton");
        if (_menuButton != null)
        {
            _menuButton.clicked += OnMenuButtonClicked;
        }
    }
    private void OnMenuButtonClicked()
    {
        // 버튼 클릭 시 실행할 코드를 여기에 작성합니다.
        CustomLogger.Log("MenuButton clicked!","yellow");
        float time = Time.timeScale;
        if (time <= 0)
        {
            
        }
    }
}

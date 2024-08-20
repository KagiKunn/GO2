using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InputBox : MonoBehaviour
{
    [SerializeField] private TMP_InputField Input;
    [SerializeField] private TextMeshProUGUI Username;
    [SerializeField] private Button Change;
    [SerializeField] private Button Quit;
    void Start()
    {
        Quit.onClick.AddListener(CloseWindow);
        Change.onClick.AddListener(ChangeNick);
    }

    void Update()
    {
        
    }

    void ChangeNick()
    {
        if (Input.text != null)
        {
            Username.text = Input.text;
            CloseWindow();
        }
    }
    
    void CloseWindow()
    {
        GameObject InputBox = GameObject.Find("InputBox");
        Input.text = null;
        InputBox.SetActive(false);
    }
}

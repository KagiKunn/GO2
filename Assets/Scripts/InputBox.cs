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
        DontDestroyOnLoad(this);
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        
    }

    void ChangeNick()
    {
        if (Input.text != null)
        {
            Username.text = Input.text;
            PlayerSyncManager.Instance.Username = Input.text;
            PlayerSyncManager.Instance.Save();
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

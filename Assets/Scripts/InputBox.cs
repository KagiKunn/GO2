using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InputBox : MonoBehaviour
{
    [SerializeField] public TMP_InputField Input;
    [SerializeField] private TextMeshProUGUI Username;
    [SerializeField] private Button Change;
    [SerializeField] private Button Quit;
    public string nowType = "Nick";
    public string beforeType = "Nick";
    public static InputBox Instance { get; private set; }
    void Start()
    {
        Quit.onClick.AddListener(CloseWindow);
    }
    
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            this.gameObject.SetActive(false);
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        if (nowType == "Nick" && nowType != beforeType)
        {
            Change.onClick.AddListener(ChangeNick);
            Change.onClick.RemoveListener(Continue);
            Change.onClick.RemoveListener(Issue);
            beforeType = nowType;
        } else if (nowType == "Continue" && nowType != beforeType)
        {
            Change.onClick.AddListener(Continue);
            Change.onClick.RemoveListener(ChangeNick);
            Change.onClick.RemoveListener(Issue);
            beforeType = nowType;
        } else if (nowType == "Issue" && nowType != beforeType)
        {
            Change.onClick.AddListener(Issue);
            Change.onClick.RemoveListener(ChangeNick);
            Change.onClick.RemoveListener(Continue);
            beforeType = nowType;
        }
    }

    void ChangeNick()
    {
        if (!string.IsNullOrEmpty(Input.text))
        {
            Username.text = Input.text;
            PlayerSyncManager.Instance.Username = Input.text;
            PlayerSyncManager.Instance.Save();
            CloseWindow();
        }
    }

    void Continue()
    {
        if (!string.IsNullOrEmpty(Input.text))
        {
            PlayerSyncManager.Instance.ChangeAccount(Input.text);
        }
    }

    public void Issue()
    {
        PlayerSyncManager.Instance.IssueCode();
    }
    
    void CloseWindow()
    {
        GameObject InputBox = GameObject.Find("InputBox");
        Input.text = null;
        InputBox.SetActive(false);
    }
}

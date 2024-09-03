using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InternalUiButton : MonoBehaviour
{
    public Button settingButton;
    public Button giveupButton;

    private bool settingActive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        settingButton.onClick.AddListener(() => SettingControl());
        giveupButton.onClick.AddListener(() => GiveupControl());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SettingControl()
    {
        SfxManager.Instance.clickSound();
        settingActive = !settingActive;
        if (settingActive)
        {
            SettingManager.Instance.SetMainCamera();
            SettingManager.Instance.gameObject.SetActive(true);
            SettingManager.Instance.LoadText();
        }
        else
        {
            SettingManager.Instance.gameObject.SetActive(false);
        }
    }
    public void GiveupControl()
    {
        SfxManager.Instance.clickSound();
        SceneManager.LoadScene("Title");
    }
}

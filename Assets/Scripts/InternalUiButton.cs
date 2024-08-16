using UnityEngine;
using UnityEngine.UI;

public class InternalUiButton : MonoBehaviour
{
    public Button settingButton;
    public GameObject settingObject;

    private bool settingActive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        settingButton.onClick.AddListener(() => SettingControl());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SettingControl()
    {
        settingActive = !settingActive;
        if (settingActive)
        {
            settingObject.SetActive(true);
        }
        else
        {
            settingObject.SetActive(false);
        }
    }
}

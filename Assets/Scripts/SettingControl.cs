using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;

#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.

public class SettingControl : MonoBehaviour
{
    private static string persistentDataPath;
    [SerializeField] private Slider masterVolSlider;
    [SerializeField] private TextMeshProUGUI masterVal;
    [SerializeField] private Slider bgmVolSlider;
    [SerializeField] private TextMeshProUGUI bgmVal;
    [SerializeField] private Slider sfxVolSlider;
    [SerializeField] private TextMeshProUGUI sfxVal;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Toggle vibrationToggle;
    [SerializeField] private TextMeshProUGUI UUID;
    [SerializeField] private TextMeshProUGUI Username;
    private string filepath;

    public static SettingControl Instance { get; private set; }
    private GameObject nickChange;
    [SerializeField] public bool IsVibrationEnabled { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }

        nickChange = GameObject.Find("SettingMenu").transform.Find("NickChange").gameObject;
        persistentDataPath = Application.persistentDataPath;
        filepath = Path.Combine(persistentDataPath, "Setting.dat");

        if (!File.Exists(filepath))
        {
            FirstSetting();
        }
        else
        {
            LoadSetting();
        }
        if (SceneManager.GetActiveScene().name.Equals("InternalAffairs"))
        {
            nickChange.SetActive(true);
            Button nickChangeButton = nickChange.GetComponent<Button>();
            nickChangeButton.onClick.AddListener(openNickChange);
        }
        else
        {
            nickChange.SetActive(false);
        }

        masterVolSlider.onValueChanged.AddListener(value => SetLevel("Master", masterVolSlider.value, masterVal, true));
        sfxVolSlider.onValueChanged.AddListener(value => SetLevel("SFX", sfxVolSlider.value, sfxVal, true));
        bgmVolSlider.onValueChanged.AddListener(value => SetLevel("BGM", bgmVolSlider.value, bgmVal, true));
        vibrationToggle.onValueChanged.AddListener(SetVibration);

        if (PlayerSyncManager.Instance != null)
        {
            if (PlayerSyncManager.Instance.UUID != null)
            {
                UUID.text = "UUID : " + PlayerSyncManager.Instance.UUID;
                //GUIUtility.systemCopyBuffer = PlayerSyncManager.Instance.UUID;
            }

            if (PlayerSyncManager.Instance.Username != null)
            {
                Username.text = PlayerSyncManager.Instance.Username;
            }
        }
    }

    public void SetVibration(bool isEnabled)
    {
        Instance.IsVibrationEnabled = isEnabled;
        SaveSetting();
    }

    private async void SetLevel(string parameter, float sliderValue, TextMeshProUGUI displayText, bool save)
    {
        float dBValue = Mathf.Log10(sliderValue) * 20;
        audioMixer.SetFloat(parameter, dBValue);

        LocalizedString localizedString = new LocalizedString
            { TableReference = "Setting", TableEntryReference = parameter + "Vol" };

        localizedString.StringChanged += (localizedText) =>
        {
            displayText.text = $"{localizedText}{Mathf.Round(sliderValue * 100)}";
        };

        if (save)
        {
            SaveSetting();
        }
    }

    private void openNickChange()
    {
        
    }
    
    private void FirstSetting()
    {
        SetSliderValueFromMixer("Master", masterVolSlider);
        SetSliderValueFromMixer("SFX", sfxVolSlider);
        SetSliderValueFromMixer("BGM", bgmVolSlider);
        Instance.IsVibrationEnabled = true;
        vibrationToggle.isOn = true;

        SaveSetting();
    }

    private async void LoadSetting()
    {
        try
        {
            using (FileStream file = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                Setting ss = (Setting)formatter.Deserialize(file);

                masterVolSlider.value = ss.master;
                bgmVolSlider.value = ss.bgm;
                sfxVolSlider.value = ss.sfx;
                vibrationToggle.isOn = ss.vibrate;

                await Task.Delay(10);

                SetLevel("Master", masterVolSlider.value, masterVal, false);
                SetLevel("BGM", bgmVolSlider.value, bgmVal, false);
                SetLevel("SFX", sfxVolSlider.value, sfxVal, false);
                Instance.IsVibrationEnabled = ss.vibrate;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load settings: {ex.Message}");
            File.Delete(filepath);
            FirstSetting();
        }
    }

    private void SaveSetting()
    {
        try
        {
            Setting ss = new Setting
            {
                master = masterVolSlider.value,
                bgm = bgmVolSlider.value,
                sfx = sfxVolSlider.value,
                vibrate = vibrationToggle.isOn
            };

            using (FileStream file = File.Create(filepath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(file, ss);
                file.Close();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save settings: {ex.Message}");
        }
    }

    private void SetSliderValueFromMixer(string parameter, Slider slider)
    {
        if (audioMixer.GetFloat(parameter, out float value))
        {
            slider.value = Mathf.Pow(10, value / 20f);
        }
    }
}
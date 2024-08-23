using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEngine.SceneManagement;

#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.

public class SettingManager : MonoBehaviour
{
    private static string persistentDataPath;
    [SerializeField] private Slider masterVolSlider;
    [SerializeField] private TextMeshProUGUI masterVal;
    [SerializeField] private Slider bgmVolSlider;
    [SerializeField] private TextMeshProUGUI bgmVal;
    [SerializeField] private Slider sfxVolSlider;
    [SerializeField] private TextMeshProUGUI sfxVal;
    [SerializeField] private Slider voiceVolSlider;
    [SerializeField] private TextMeshProUGUI voiceVal;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Toggle vibrationToggle;
    [SerializeField] private TextMeshProUGUI UUID;
    [SerializeField] private TextMeshProUGUI Username;
    [SerializeField] private GameObject InputBoxUI;
    private string filepath;

    public static SettingManager Instance { get; private set; }
    private GameObject nickChange;
    private GameObject ContinueGet;
    private GameObject ContinueInput;
    public TMP_InputField inputField;
    private LocalizedString localizedString;
    private TMP_Text plt;
    [SerializeField] public bool IsVibrationEnabled { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }

        nickChange = gameObject.transform.Find("NickChange").gameObject;
        ContinueGet = gameObject.transform.Find("ContinueGet").gameObject;
        ContinueInput = gameObject.transform.Find("ContinueInput").gameObject;

        persistentDataPath = Application.persistentDataPath;
        filepath = Path.Combine(persistentDataPath, "Setting.dat");
        plt = inputField.placeholder as TMP_Text;

        if (!File.Exists(filepath))
        {
            FirstSetting();
        }
        else
        {
            LoadSetting();
        }

        masterVolSlider.onValueChanged.AddListener(value => SetLevel("Master", masterVolSlider.value, masterVal, true));
        sfxVolSlider.onValueChanged.AddListener(value => SetLevel("SFX", sfxVolSlider.value, sfxVal, true));
        bgmVolSlider.onValueChanged.AddListener(value => SetLevel("BGM", bgmVolSlider.value, bgmVal, true));
        voiceVolSlider.onValueChanged.AddListener(value => SetLevel("Voice", voiceVolSlider.value, voiceVal, true));
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

    public void SetMainCamera()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
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

    public void openNickChange()
    {
        InputBox.Instance.nowType = "Nick";
        SetPlaceholderText("NickChange");
        InputBoxUI.SetActive(true);
    }

    public void openIssue()
    {
        InputBox.Instance.nowType = "Issue";
        SetPlaceholderText("Continous");
        InputBoxUI.SetActive(true);
    }
    public void openContinue()
    {
        InputBox.Instance.nowType = "Continue";
        SetPlaceholderText("Continous");
        InputBoxUI.SetActive(true);
    }
    
    private void FirstSetting()
    {
        SetSliderValueFromMixer("Master", masterVolSlider);
        SetSliderValueFromMixer("SFX", sfxVolSlider);
        SetSliderValueFromMixer("BGM", bgmVolSlider);
        SetSliderValueFromMixer("Voice", voiceVolSlider);
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
                SetLevel("Voice", voiceVolSlider.value, voiceVal, false);
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

    public void LoadText()
    {
        if (SceneManager.GetActiveScene().name.Equals("InternalAffairs"))
        {
            nickChange.SetActive(true);
            ContinueGet.SetActive(true);
            ContinueInput.SetActive(true);
            Button nickChangeButton = nickChange.GetComponent<Button>();
            nickChangeButton.onClick.AddListener(openNickChange);
            Button ContinueGetBtn = ContinueGet.GetComponent<Button>();
            ContinueGetBtn.onClick.AddListener(openIssue);
            Button ContinueInputBtn = ContinueInput.GetComponent<Button>();
            ContinueInputBtn.onClick.AddListener(openContinue);
        }
        else
        {
            nickChange.SetActive(false);
            ContinueGet.SetActive(false);
            ContinueInput.SetActive(false);
        }
        SetLevel("Master", masterVolSlider.value, masterVal, false);
        SetLevel("BGM", bgmVolSlider.value, bgmVal, false);
        SetLevel("SFX", sfxVolSlider.value, sfxVal, false);
        SetLevel("Voice", voiceVolSlider.value, voiceVal, false);
        if (PlayerSyncManager.Instance != null)
        {
            if (PlayerSyncManager.Instance.UUID != null)
            {
                UUID.text = "UUID : " + PlayerSyncManager.Instance.UUID;
            }

            if (PlayerSyncManager.Instance.Username != null)
            {
                Username.text = PlayerSyncManager.Instance.Username;
            }
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
                vibrate = vibrationToggle.isOn,
                voice = voiceVolSlider.value
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
    
    private void SetPlaceholderText(string tableEntryReference)
    {
        // 이전 이벤트 구독 해제
        if (localizedString != null && plt != null)
        {
            localizedString.StringChanged -= UpdatePlaceholderText;
        }

        // 새로운 LocalizedString 생성
        localizedString = new LocalizedString
        {
            TableReference = "UI",
            TableEntryReference = tableEntryReference
        };

        if (plt != null)
        {
            // 새로운 이벤트 구독
            localizedString.StringChanged += UpdatePlaceholderText;
        }
    }

    private void UpdatePlaceholderText(string localizedText)
    {
        if (plt != null)
        {
            plt.text = localizedText;
        }
    }
    private void OnDisable()
    {
        // 오브젝트가 파괴될 때 이벤트 구독 해제
        if (localizedString != null && plt != null)
        {
            localizedString.StringChanged -= UpdatePlaceholderText;
        }
    }
    
    private void OnDestroy()
    {
        // 오브젝트가 파괴될 때 이벤트 구독 해제
        if (localizedString != null && plt != null)
        {
            localizedString.StringChanged -= UpdatePlaceholderText;
        }
    }
}
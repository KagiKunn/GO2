using System;

using TMPro;

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

public class SoundControl : MonoBehaviour {
	private static string persistentDataPath;
	[SerializeField] private Slider masterVolSlider;
	[SerializeField] private TextMeshProUGUI masterVal;
	[SerializeField] private Slider bgmVolSlider;
	[SerializeField] private TextMeshProUGUI bgmVal;
	[SerializeField] private Slider sfxVolSlider;
	[SerializeField] private TextMeshProUGUI sfxVal;
	[SerializeField] private AudioMixer audioMixer;
	private string filepath;

	private void Awake() {
		persistentDataPath = Application.persistentDataPath;
		filepath = Path.Combine(persistentDataPath, "Setting.dat");

		if (!File.Exists(filepath)) {
			FirstSetting();
		} else {
			LoadSetting();
		}

		masterVolSlider.onValueChanged.AddListener(value => SetLevel("Master", masterVolSlider.value, masterVal, true));
		sfxVolSlider.onValueChanged.AddListener(value => SetLevel("SFX", sfxVolSlider.value, sfxVal, true));
		bgmVolSlider.onValueChanged.AddListener(value => SetLevel("BGM", bgmVolSlider.value, bgmVal, true));
	}

	private void SetLevel(string parameter, float sliderValue, TextMeshProUGUI displayText, bool save) {
		float dBValue = Mathf.Log10(sliderValue) * 20;
		audioMixer.SetFloat(parameter, dBValue);
		displayText.text = $"{parameter}: {Mathf.Round(sliderValue * 100)}";

		if (save) {
			SaveSetting();
		}
	}

	private void FirstSetting() {
		SetSliderValueFromMixer("Master", masterVolSlider);
		SetSliderValueFromMixer("SFX", sfxVolSlider);
		SetSliderValueFromMixer("BGM", bgmVolSlider);

		SaveSetting();
	}

	private async void LoadSetting() {
		try {
			using (FileStream file = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.None)) {
				BinaryFormatter formatter = new BinaryFormatter();
				SoundSetting ss = (SoundSetting)formatter.Deserialize(file);

				masterVolSlider.value = ss.master;
				bgmVolSlider.value = ss.bgm;
				sfxVolSlider.value = ss.sfx;

				await Task.Delay(10);

				SetLevel("Master", masterVolSlider.value, masterVal, false);
				SetLevel("BGM", bgmVolSlider.value, bgmVal, false);
				SetLevel("SFX", sfxVolSlider.value, sfxVal, false);

				Debug.LogWarning("Master:" + ss.master);
				Debug.LogWarning("BGM:" + ss.bgm);
				Debug.LogWarning("SFX:" + ss.sfx);
			}
		} catch (Exception ex) {
			Debug.LogError($"Failed to load settings: {ex.Message}");
		}
	}

	private void SaveSetting() {
		try {
			SoundSetting ss = new SoundSetting {
				master = masterVolSlider.value,
				bgm = bgmVolSlider.value,
				sfx = sfxVolSlider.value
			};

			using (FileStream file = File.Create(filepath)) {
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(file, ss);
				file.Close();
			}
		} catch (Exception ex) {
			Debug.LogError($"Failed to save settings: {ex.Message}");
		}
	}

	private void SetSliderValueFromMixer(string parameter, Slider slider) {
		if (audioMixer.GetFloat(parameter, out float value)) {
			slider.value = Mathf.Pow(10, value / 20f);
		}
	}
}
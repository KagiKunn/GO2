using System;

using TMPro;

using UnityEngine;
using UnityEngine.Audio;

public class DefenseInit : MonoBehaviour {
	public AudioClip soundClip;
	private AudioSource audioSource;

	public AudioMixerGroup bgmMixerGroup; // 배경음악용 믹서 그룹

	private int startGold;
	private int earnGold;
	private int castleHealth;
	private int cooldown;

	public int soul;
	public int currentGold;

	public int extraGold;
	public int extraCool;

	public int StartGold {
		get => startGold;

		set => startGold = value;
	}

	public int EarnGold {
		get => earnGold;

		set => earnGold = value;
	}

	public int CastleHealth {
		get => castleHealth;

		set => castleHealth = value;
	}

	public int Cooldown {
		get => cooldown;

		set => cooldown = value;
	}

	public int CurrentGold {
		get => currentGold;

		set => currentGold = value;
	}

	public int Soul {
		get => soul;

		set => soul = value;
	}

	public int extraGold1 {
		get => extraGold;

		set => extraGold = value;
	}

	public int extraCool1 {
		get => extraCool;

		set => extraCool = value;
	}

	private void Awake() {
		EarnGoldSetup();

        CastleHealthSetup();
        
        PlayerLocalManager.Instance.Save();
    }

	private void Start() {
		audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = soundClip;
		audioSource.loop = true;
		audioSource.outputAudioMixerGroup = bgmMixerGroup;
		PlaySound();
	}

	public void PlaySound() {
		if (audioSource != null && soundClip != null) {
			audioSource.Play();
		}
	}

	void EarnGoldSetup() {
		switch (earnGold) {
			case 1:
				extraGold1 = 10;

				break;
			case 2:
				extraGold1 = 20;

				break;
			case 3:
				extraGold1 = 30;

				break;
			case 4:
				extraGold1 = 40;

				break;
			default:
				extraGold1 = 0;

				break;
		}
	}

    void CastleHealthSetup()
    {
        int castleExtraHp = earnGold switch
        {
            1 => 500,
            2 => 1000,
            3 => 1500,
            4 => 2000,
            _ => 0
        };
        PlayerLocalManager.Instance.lCastleExtraHp = castleExtraHp;
    }

	void CoolDownSetup() {
		switch (cooldown) {
			case 1:
				extraCool = 10;

				break;
			case 2:
				extraCool = 20;

				break;
			case 3:
				extraCool = 30;

				break;
			case 4:
				extraCool = 40;

				break;
			default:
				extraCool = 0;

				break;
		}
	}

	private void Update() {
		GameObject.Find("Gold").GetComponent<TMP_InputField>().text = currentGold.ToString();
	}

	private void OnDisable() {
		PlayerLocalManager.Instance.lMoney = currentGold;
		PlayerLocalManager.Instance.lPoint = soul;
		PlayerLocalManager.Instance.Save();
	}
}
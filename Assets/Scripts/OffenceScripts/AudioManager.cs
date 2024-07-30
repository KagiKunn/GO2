using System;

using UnityEngine;

using static AudioManager;

using Random = UnityEngine.Random;

#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.

public class AudioManager : MonoBehaviour {
	private static AudioManager instance = null;

	[Header("# BGM")]
	[SerializeField] private AudioClip bgmClip;

	[SerializeField] private float bgmVolmue;
	private AudioSource bgmPlayer;
	private AudioHighPassFilter bgmEffect;

	[Header("# SFX")]
	[SerializeField] private AudioClip[] sfxClips;

	[SerializeField] private float sfxVolmue;
	[SerializeField] private int channels;
	private AudioSource[] sfxPlayers;
	private int channelIndex;

	public enum Sfx {
		Dead,
		Hit,
		LevelUp = 3,
		Lose,
		Melee,
		Range = 7,
		Select,
		Win
	}

	private void Awake() {
		if (instance == null) {
			instance = this;

			Initialize();
		}
	}

	public static AudioManager Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<AudioManager>();

				if (instance == null) {
					return null;
				} else {
					instance.Initialize();
				}
			}

			return instance;
		}
	}

	private void Initialize() {
		// 배경음 플레이어 초기화
		GameObject bgmObject = new GameObject("Bgm");

		bgmObject.transform.parent = transform;

		bgmPlayer = bgmObject.AddComponent<AudioSource>();

		bgmPlayer.playOnAwake = false;
		bgmPlayer.loop = true;
		bgmPlayer.volume = bgmVolmue;
		bgmPlayer.clip = bgmClip;

		bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

		// 효과음 플레이어 초기화
		GameObject sfxObject = new GameObject("SfxPlayer");

		sfxObject.transform.parent = transform;
		sfxPlayers = new AudioSource[channels];

		for (int i = 0; i < sfxPlayers.Length; i++) {
			sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();

			sfxPlayers[i].playOnAwake = false;
			sfxPlayers[i].volume = sfxVolmue;
			sfxPlayers[i].bypassListenerEffects = true;
		}
	}

	public void PlayBgm(bool isPlay) {
		if (isPlay) {
			bgmPlayer.Play();
		} else {
			bgmPlayer.Stop();
		}
	}

	public void EffectBgm(bool isPlay) {
		bgmEffect.enabled = isPlay;
	}

	public void PlaySfx(Sfx sfx) {
		for (int i = 0; i < sfxPlayers.Length; i++) {
			int loopIndex = (i + channelIndex) % sfxPlayers.Length;

			if (sfxPlayers[loopIndex].isPlaying)
				continue;

			int ranIndex = 0;

			if (sfx == Sfx.Hit || sfx == Sfx.Melee) {
				ranIndex = Random.Range(0, 2);
			}

			channelIndex = loopIndex;

			sfxPlayers[loopIndex].clip = sfxClips[(int)sfx + ranIndex];
			sfxPlayers[loopIndex].Play();

			break;
		}
	}
}
using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Tilemaps;

public class CastleWall : MonoBehaviour {
	//CastleWall 클래스는 성벽이 데미지를 받을 때 CastleWallManager에 처리를 위임하고, 성벽 오브젝트를 삭제하는 역할만 담당
	private Tilemap castleTile;
	private Color originColor;

	public AudioClip soundClip;
	private AudioSource attackAudioSource;

	private void Awake() {
		castleTile = GetComponent<Tilemap>();
		originColor = castleTile.color;
	}

	private void Start() {
		attackAudioSource = gameObject.AddComponent<AudioSource>();
		attackAudioSource.clip = soundClip;
		attackAudioSource.volume /= 2;
	}

	public void PlaySound() {
		if (attackAudioSource != null && soundClip != null) {
			attackAudioSource.Play();
		}
	}

	public void TakeDamage(float damage) {
		string wallTag = gameObject.tag;
		CastleWallManager.Instance.ApplyDamage(damage);

		PlaySound();

		CustomLogger.Log($"Damage applied to {wallTag}");

		if (CastleWallManager.Instance.GetHealth() <= 0) {
			CustomLogger.Log($"{wallTag} destroyed.");
			Destroy(gameObject); // 성벽 오브젝트 삭제
		}
	}

	public void ChangeWallColor(bool activateShield) {
		if (activateShield) {
			castleTile.color = Color.cyan;
		} else {
			castleTile.color = originColor;
		}
	}
}
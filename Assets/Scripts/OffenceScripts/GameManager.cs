using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

#pragma warning disable CS0414, CS0618 // 필드가 대입되었으나 값이 사용되지 않습니다

public class GameManager : MonoBehaviour {
	private static GameManager instance = null;

	[Header("# Game Control")]
	[SerializeField] private bool isLive;

	[SerializeField] private float gameTime;
	[SerializeField] private float maxGameTime = 2 * 10f;

	[Header("# Player Info")]
	[SerializeField] private int playerId;

	[SerializeField] private float health;
	[SerializeField] private float maxHealth = 100;
	[SerializeField] private int level;
	[SerializeField] private int kill;
	[SerializeField] private int exp;
	[SerializeField] private int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };

	[Header("# Game Object")]
	[SerializeField] private Player player;

	[SerializeField] private PoolManager poolManager;
	[SerializeField] private LevelUp uiLevelUp;
	[SerializeField] private Result uiResult;
	[SerializeField] private GameObject enemyCleaner;

	private void Awake() {
		if (instance == null) {
			instance = this;

			DontDestroyOnLoad(this.gameObject);
		} else if (instance != this) {
			Destroy(this.gameObject);
		}
		
		CustomLogger.Log(playerId);
	}

	public void GameStart(int id) {
		playerId = id;

		health = maxHealth;

		player.gameObject.SetActive(true);

		uiLevelUp.Select(playerId % 2);

		Resume();

		AudioManager.Instance.PlayBgm(true);
		AudioManager.Instance.PlaySfx(AudioManager.Sfx.Select);
	}

	public void GameOver() {
		StartCoroutine(GameOverRoutine());
	}

	IEnumerator GameOverRoutine() {
		isLive = false;

		yield return new WaitForSeconds(0.5f);

		uiResult.gameObject.SetActive(true);

		uiResult.Lose();

		Stop();

		AudioManager.Instance.PlayBgm(false);
		AudioManager.Instance.PlaySfx(AudioManager.Sfx.Lose);
	}

	public void GameVictory() {
		StartCoroutine(GameVictoryRoutine());
	}

	IEnumerator GameVictoryRoutine() {
		isLive = false;
		enemyCleaner.SetActive(true);

		yield return new WaitForSeconds(0.5f);

		uiResult.gameObject.SetActive(true);

		uiResult.Win();

		Stop();

		AudioManager.Instance.PlayBgm(false);
		AudioManager.Instance.PlaySfx(AudioManager.Sfx.Win);
	}

	public void GameRetry() {
		Destroy(this.gameObject);

		SceneManager.LoadScene("Offence");
	}

	private void Update() {
		if (!isLive) return;

		gameTime += Time.deltaTime;

		if (gameTime > maxGameTime) {
			gameTime = maxGameTime;

			GameVictory();
		}
	}

	private void Initialize() {
		if (player == null) {
			player = FindObjectOfType<Player>();

			if (player == null) {
				CustomLogger.Log("Player instance not found", "red");
			} else {
				CustomLogger.Log("Player instance found and assigned");
			}
		}

		if (poolManager == null) {
			poolManager = FindObjectOfType<PoolManager>();

			if (poolManager == null) {
				CustomLogger.Log("PoolManager instance not found", "red");
			} else {
				CustomLogger.Log("PoolManager instance found and assigned");
			}
		}
	}

	public void GetExp() {
		if (!isLive) return;

		exp++;

		if (exp == nextExp[Mathf.Min(level, nextExp.Length - 1)]) {
			level++;
			exp = 0;

			maxHealth += 10;

			uiLevelUp.Show();
		}
	}

	public static GameManager Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<GameManager>();

				if (instance == null) {
					CustomLogger.Log("No Singleton Object", "red");

					return null;
				} else {
					instance.Initialize();
				}
			}

			return instance;
		}
	}

	public void Stop() {
		isLive = false;

		Time.timeScale = 0;
	}

	public void Resume() {
		isLive = true;

		Time.timeScale = 1;
	}

	public Player Player => player;
	public PoolManager PoolManager => poolManager;

	public float GameTime => gameTime;
	public float MaxGameTime => maxGameTime;

	public bool IsLive => isLive;

	public float Health {
		get => health;

		set => health = value;
	}

	public float MaxHealth {
		get => maxHealth;

		set => maxHealth = value;
	}

	public int Level => level;

	public int Kill {
		get => kill;

		set => kill = value;
	}

	public int Exp {
		get => exp;

		set => exp = value;
	}

	public int[] NextExp {
		get => nextExp;
	}

	public int PlayerId => playerId;
}
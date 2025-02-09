using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using InternalAffairs;

using Unity.Cinemachine;

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#pragma warning disable CS0108, CS0114

#pragma warning disable CS0414, CS0618 // 필드가 대입되었으나 값이 사용되지 않습니다

public class GameManager : MonoBehaviour {
	private static GameManager instance = null;

	[Header("# Game Control")] [SerializeField]
	private bool isLive;

	[SerializeField] private float gameTime;
	[SerializeField] private float maxGameTime = 2 * 10f;

	[Header("# Player Info")] [SerializeField]
	private int playerId;

	[SerializeField] private float health;
	[SerializeField] private float maxHealth = 100;
	[SerializeField] private float attackSpeed;
	[SerializeField] private float attackDamage;
	[SerializeField] private int level;
	[SerializeField] private int kill;
	[SerializeField] private int exp;
	[SerializeField] private int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };

	[Header("# Game Object")] [SerializeField]
	private Player[] players;

	[SerializeField] private PoolManager poolManager;
	[SerializeField] private LevelUp uiLevelUp;
	[SerializeField] private Result uiResult;
	[SerializeField] private GameObject enemyCleaner;
	[SerializeField] private CinemachineCamera camera;

	private List<string> selectedHeroes = new List<string>();
	private List<HeroData> heroDatas = new List<HeroData>();

	private string filePath;

	private Hand hand;
	private Weapon weapon;

	public enum HeroNames {
		Dummy,
		KKS01,
		Novice,
		Knight,
		DualWeapon,
		Elf,
		LSH01,
		CHS01
	}

	public HeroNames heroNames;

	private void Awake() {
		if (instance == null) {
			instance = this;

			DontDestroyOnLoad(this.gameObject);
		} else if (instance != this) {
			Destroy(this.gameObject);
		}

		HeroList[] heroList = PlayerLocalManager.Instance.lHeroeList;

		for (int i = 0; i < heroList.Length; i++) {
			if (heroList[i].Item3 > 0) {
				string hero = heroList[i].Item1;

				if (hero != null) {
					selectedHeroes.Add(hero);
					heroDatas.Add(HeroManager.Instance.heroDataList.Find(h => h.Name == heroList[i].Item1));
				}
			}
		}

		// foreach (string selectedHero in selectedHeroes) {
		//	CustomLogger.Log(selectedHero);
		// }
	}

	public void GameStart(Text text) {
		for (int i = 0; i < selectedHeroes.Count; i++) {
			if (text.text == selectedHeroes[i]) {
				HeroNames heroEnum;

				if (Enum.TryParse(selectedHeroes[i], out heroEnum)) {
					players[(int)heroEnum].gameObject.SetActive(true);
					playerId = (int)heroEnum;

					maxHealth = heroDatas[i].OffenceHP;
					attackSpeed = heroDatas[i].OffenceAttackSpeed;
					attackDamage = heroDatas[i].OffenceAttack / 100f;

					camera.Target.TrackingTarget = players[(int)heroEnum].gameObject.transform;
				} else {
					CustomLogger.Log("Invalid hero name: " + selectedHeroes[i]);
				}
			}
		}

		health = maxHealth;

		CustomLogger.Log(playerId);

		uiLevelUp.Select(playerId + 5);
		Resume();
		AudioManager.Instance.PlayBgm(true);
		AudioManager.Instance.PlaySfx(AudioManager.Sfx.Select);
	}

	public void GameOver() {
		StartCoroutine(GameOverRoutine());
	}

	IEnumerator GameOverRoutine() {
		isLive = false;
		enemyCleaner.SetActive(true);

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
		SceneManager.LoadScene("InternalAffairs");
	}

	public void GameWin() {
		PlayerLocalManager.Instance.lNextEnemy = true;

		Time.timeScale = 1;
		SceneManager.LoadScene("Reward");
	}

	private void Update() {
		if (!isLive) {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				PlayerLocalManager.Instance.lMoney++;
				PlayerLocalManager.Instance.Save();
				SceneManager.LoadScene("InternalAffairs");
			}

			return;
		}

		gameTime += Time.deltaTime;

		if (gameTime > maxGameTime) {
			gameTime = maxGameTime;

			GameVictory();
		}
	}

	private void Initialize() {
		if (players == null || players.Length == 0) {
			players = FindObjectsOfType<Player>();

			if (players == null || players.Length == 0) {
				CustomLogger.Log("Player instances not found", "red");
			} else {
				CustomLogger.Log("Player instances found and assigned");
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
			health += 10;
			maxHealth += 10;

			uiLevelUp.Show(playerId + 5);
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

	public Player[] Player => players;
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

	public float AttackSpeed => attackSpeed;

	public float AttackDamage => attackDamage;

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
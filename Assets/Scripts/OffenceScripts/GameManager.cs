using System;

using UnityEngine;
using UnityEngine.Animations;

#pragma warning disable CS0414 // 필드가 대입되었으나 값이 사용되지 않습니다

public class GameManager : MonoBehaviour {
	#pragma warning disable CS0618
	private static GameManager instance = null;

	[Header("# Game Control")]
	[SerializeField] private bool isLive;

	[SerializeField] private float gameTime;
	[SerializeField] private float maxGameTime = 2 * 10f;

	[Header("# Player Info")]
	[SerializeField] private int health;

	[SerializeField] private int maxHealth = 100;
	[SerializeField] private int level;
	[SerializeField] private int kill;
	[SerializeField] private int exp;
	[SerializeField] private int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };

	[Header("# Game Object")]
	[SerializeField] private Player player;

	[SerializeField] private PoolManager poolManager;
	[SerializeField] private LevelUp uiLevelUp;

	private void Awake() {
		if (instance == null) {
			instance = this;

			DontDestroyOnLoad(this.gameObject);
		} else if (instance != this) {
			Destroy(this.gameObject);
		}
	}

	private void Start() {
		health = maxHealth;

		// 임시 스크립트(첫번째 캐릭터 선택)
		uiLevelUp.Select(0);
	}

	private void Update() {
		if (!isLive) return;

		gameTime += Time.deltaTime;

		if (gameTime > maxGameTime) {
			gameTime = maxGameTime;
		}
	}

	private void Initialize() {
		if (player == null) {
			player = FindObjectOfType<Player>();

			if (player == null) {
				CustomLogger.LogError("Player instance not found");
			} else {
				CustomLogger.Log("Player instance found and assigned");
			}
		}

		if (poolManager == null) {
			poolManager = FindObjectOfType<PoolManager>();

			if (poolManager == null) {
				CustomLogger.LogError("PoolManager instance not found");
			} else {
				CustomLogger.Log("PoolManager instance found and assigned");
			}
		}
	}

	public void GetExp() {
		exp++;

		if (exp == nextExp[Mathf.Min(level, nextExp.Length - 1)]) {
			level++;
			exp = 0;

			uiLevelUp.Show();
		}
	}

	public static GameManager Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<GameManager>();

				if (instance == null) {
					CustomLogger.LogError("No Singleton Object");

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

	public int Health {
		get => health;

		set => health = value;
	}

	public int MaxHealth {
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
}
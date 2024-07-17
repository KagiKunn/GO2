using System;

using UnityEngine;

public class GameManager : MonoBehaviour {
	#pragma warning disable CS0618
	private static GameManager instance;

	[SerializeField]
	private Player player;

	public static GameManager Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType(typeof(GameManager)) as GameManager;

				if (instance == null) CustomLogger.LogError("No Singleton Object");
			}

			return instance;
		}
	}

	public Player Player {
		get { return player != null ? player : null; }
	}

	private void Awake() {
		if (instance == null) instance = this;
		else if (instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

		if (player == null) {
			player = FindObjectOfType<Player>();

			if (player == null) CustomLogger.LogError("Player instance not found");
			else CustomLogger.Log("Player instance found and assigned");
		}
	}
}
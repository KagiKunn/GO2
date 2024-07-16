using System;

using UnityEngine;

public class GameManager : MonoBehaviour {
	#pragma warning disable CS0618
	private static GameManager instance;

	[SerializeField]
	private Player player;

	public static GameManager Instance {
		get {
			if (!instance) {
				instance = FindObjectOfType(typeof(GameManager)) as GameManager;

				if (instance == null) Debug.Log("No Singleton obj");
			}

			return instance;
		}
	}

	public Player Player {
		get {
			if (player != null) return player;

			return null;
		}

		set { player = value; }
	}

	private void Awake() {
		if (instance == null) instance = this;
		else if (instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}
}
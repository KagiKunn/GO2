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

				if (instance == null) Debug.Log("No Singleton obj 123123124124124123123123");
			}

			return instance;
		}
	}

	public Player Player {
		get {
			if (player != null) return player;

			return null;
		}
	}

	private void Awake() {
		if (instance == null){ instance = this;
			Debug.Log("인스턴스 생성");}
		else if (instance != this){ Destroy(gameObject);
			Debug.Log("Destroied");}

		player = GetComponent<Player>();

		DontDestroyOnLoad(gameObject);
	}
}
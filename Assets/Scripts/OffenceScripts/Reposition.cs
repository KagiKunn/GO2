using System;

using UnityEngine;

using Random = UnityEngine.Random;

#pragma warning disable CS0108, CS0114

public class Reposition : MonoBehaviour {
	private Collider2D collider2D;

	private GameManager gameManager;
	private Player player;

	private void Awake() {
		collider2D = GetComponent<Collider2D>();

		gameManager = GameManager.Instance;
		player = gameManager.Player[gameManager.PlayerId];
	}

	private void Update() {
		if (player.gameObject.name != "Dummy") return;

		player = gameManager.Player[gameManager.PlayerId];
	}

	private void OnTriggerExit2D(Collider2D other) {
		if (!gameManager.IsLive) return;

		if (!other.CompareTag("Area")) return;

		Vector3 playerPosition = player.transform.position;
		Vector3 myPosition = transform.position;

		// float directionX = playerPosition.x - myPosition.x;
		// float directionY = playerPosition.y - myPosition.y;
		//
		// float diffX = Mathf.Abs(directionX);
		// float diffY = Mathf.Abs(directionY);
		//
		// directionX = directionX > 0 ? 1 : -1;
		// directionY = directionY > 0 ? 1 : -1;

		switch (transform.tag) {
			case "Ground":
				float diffX = playerPosition.x - myPosition.x;
				float diffY = playerPosition.y - myPosition.y;

				float directionX = diffX < 0 ? -1 : 1;
				float directionY = diffY < 0 ? -1 : 1;

				diffX = Mathf.Abs(diffX);
				diffY = Mathf.Abs(diffY);

				if (diffX > diffY) transform.Translate(Vector3.right * directionX * 80);
				else if (diffX < diffY) transform.Translate(Vector3.up * directionY * 80);
				else transform.Translate(directionX * 80, directionY * 80, 0);

				break;
			case "Enemy":
				if (collider2D.enabled) {
					Vector3 distance = playerPosition - myPosition;
					Vector3 random = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);

					transform.Translate(random + distance * 2);
				}
				// if (diffX > diffY)
				// 	transform.Translate(Vector3.right * directionX * 20 + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f));
				// else if (diffX < diffY)
				// 	transform.Translate(Vector3.up * directionY * 20 + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f));

				break;
		}
	}
}
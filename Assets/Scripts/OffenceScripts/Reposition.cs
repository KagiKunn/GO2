using System;

using UnityEngine;

using Random = UnityEngine.Random;

#pragma warning disable CS0108, CS0114

public class Reposition : MonoBehaviour {
	private Collider2D collider2D;
	private Player playerInstance;

	private void Awake() {
		collider2D = GetComponent<Collider2D>();
	}

	private void OnTriggerExit2D(Collider2D other) {
		if (!other.CompareTag("Area")) return;

		if (GameManager.Instance != null) {
			playerInstance = GameManager.Instance.Player;
		}

		if (playerInstance == null) {
			CustomLogger.Log("Player instance is null", "red");

			return;
		}

		Vector3 playerPosition = playerInstance.transform.position;
		Vector3 myPosition = transform.position;

		// float directionX = playerPosition.x - myPosition.x;
		// float directionY = playerPosition.y - myPosition.y;
		//
		// float diffX = Mathf.Abs(directionX);
		// float diffY = Mathf.Abs(directionY);
		//
		// directionX = directionX > 0 ? 1 : -1;
		// directionY = directionY > 0 ? 1 : -1;

		float diffX = Mathf.Abs(playerPosition.x - myPosition.x);
		float diffY = Mathf.Abs(playerPosition.y - myPosition.y);

		Vector3 playerDirectionVector3 = playerInstance.InputVector2;

		float directionX = playerDirectionVector3.x < 0 ? -1 : 1;
		float directionY = playerDirectionVector3.y < 0 ? -1 : 1;

		Vector3 playerDirection = playerInstance.InputVector2;

		switch (transform.tag) {
			case "Ground":
				if (diffX > diffY) transform.Translate(Vector3.right * directionX * 40);
				else if (diffX < diffY) transform.Translate(Vector3.up * directionY * 40);

				break;
			case "Enemy":
				if (collider2D.enabled)
					transform.Translate(playerDirectionVector3 * 20 + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f));

				// if (diffX > diffY)
				// 	transform.Translate(Vector3.right * directionX * 20 + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f));
				// else if (diffX < diffY)
				// 	transform.Translate(Vector3.up * directionY * 20 + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f));

				break;
		}
	}
}
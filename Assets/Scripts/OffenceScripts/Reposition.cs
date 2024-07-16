using System;

using UnityEngine;

public class Reposition : MonoBehaviour {
	private void OnTriggerExit2D(Collider2D other) {
		if (!other.CompareTag("Area")) return;

		Player playerInstance = GameManager.Instance.Player;

		if (playerInstance == null) return;

		Vector3 playerPosition = playerInstance.transform.position;
		Vector3 myPosition = transform.position;

		float diffX = Mathf.Abs(playerPosition.x - myPosition.x);
		float diffY = Mathf.Abs(playerPosition.y - myPosition.y);

		Vector3 playerDirection = playerInstance.InputVec;

		float directionX = playerDirection.x < 0 ? -1 : 1;
		float directionY = playerDirection.y < 0 ? -1 : 1;

		switch (transform.tag) {
			case "Ground":
				if (diffX > diffY) transform.Translate(Vector3.right * directionX * 40);
				else if (diffX < diffY) transform.Translate(Vector3.up * directionY * 40);

				break;
			case "Enemy":
				break;
		}
	}
}
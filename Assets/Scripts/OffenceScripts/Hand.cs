using System;

using UnityEngine;

public class Hand : MonoBehaviour {
	[SerializeField] private bool isLeft;
	[SerializeField] private SpriteRenderer spriteRenderer;

	private SpriteRenderer playerSprite;
	private Player player;

	private Vector3 rightPosition = new Vector3(0.35f, -0.15f, 0);
	private Vector3 reverseRightPosition = new Vector3(-0.15f, -0.15f, 0);

	private Quaternion leftRotaion = Quaternion.Euler(0, 0, -35);
	private Quaternion reverseLeftRotaion = Quaternion.Euler(0, 0, -135);

	private void Awake() {
		playerSprite = GetComponentsInParent<SpriteRenderer>()[1];
		player = GameManager.Instance.Player;
	}

	private void LateUpdate() {
		bool isReverse = playerSprite.flipX;

		if (isLeft) { // 근접 무기
			transform.localRotation = isReverse ? reverseLeftRotaion : leftRotaion;

			spriteRenderer.flipY = isReverse;
			spriteRenderer.sortingOrder = isReverse ? 4 : 6;
		} else if (player.Scanner.NearestTarget) {
			Vector3 targetPos = player.Scanner.NearestTarget.position;
			Vector3 dir = targetPos - transform.position;

			transform.localRotation = Quaternion.FromToRotation(Vector3.right, dir);

			bool isRotA = transform.localRotation.eulerAngles.z > 90 && transform.localRotation.eulerAngles.z < 270;
			bool isRotB = transform.localRotation.eulerAngles.z < -90 && transform.localRotation.eulerAngles.z > -270;

			spriteRenderer.flipY = isRotA || isRotB;
			
			spriteRenderer.sortingOrder = 6;
		}
	}

	public SpriteRenderer SpriteRenderer {
		get => spriteRenderer;

		set => spriteRenderer = value;
	}
}
using System;

using UnityEngine;

public class Hand : MonoBehaviour {
	[SerializeField] private bool isLeft;
	[SerializeField] private SpriteRenderer spriteRenderer;

	private SpriteRenderer playerSprite;

	private GameManager gameManager;
	private Player player;

	private Vector3 rightPosition = new Vector3(0.35f, -0.15f, 0);
	private Vector3 reverseRightPosition = new Vector3(-0.15f, -0.15f, 0);

	private Quaternion leftRotaion = Quaternion.Euler(0, 0, -35);
	private Quaternion reverseLeftRotaion = Quaternion.Euler(0, 0, -135);

	private void Awake() {
		gameManager = GameManager.Instance;

		playerSprite = GetComponentsInParent<SpriteRenderer>()[1];
		player = gameManager.Player[gameManager.PlayerId];
		
		CustomLogger.Log(playerSprite.name);
		CustomLogger.Log(player.gameObject.name);
	}

	private void Update() {
		if (player.gameObject.name != "Dummy") return;

		playerSprite = GetComponentsInParent<SpriteRenderer>()[1];
		player = gameManager.Player[gameManager.PlayerId];
		
		CustomLogger.Log(playerSprite.name);
		CustomLogger.Log(player.gameObject.name);
	}

	private void LateUpdate() {
		bool isReverse = playerSprite.flipX;

		if (isLeft) { // 근접 무기
			transform.localRotation = isReverse ? reverseLeftRotaion : leftRotaion;

			spriteRenderer.flipY = isReverse;
			spriteRenderer.sortingOrder = isReverse ? 14 : 25;
		} else if (player.Scanner.NearestTarget && player.Scanner.NearestTarget != null) {
			Vector3 targetPos = player.Scanner.NearestTarget.position;
			Vector3 dir = targetPos - transform.position;

			// 회전 각도를 직접 계산하여 z축 회전을 적용합니다.
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

			// 각도를 직접 적용하여 회전시킵니다.
			transform.rotation = Quaternion.Euler(0, 0, angle);

			bool isRotA = angle > 90 && angle < 270;
			bool isRotB = angle < -90 && angle > -270;

			spriteRenderer.flipY = isRotA || isRotB;

			spriteRenderer.sortingOrder = 25;
		}
	}

	public SpriteRenderer SpriteRenderer {
		get => spriteRenderer;

		set => spriteRenderer = value;
	}
}
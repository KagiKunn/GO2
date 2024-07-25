using System;

using UnityEngine;
using UnityEngine.InputSystem;

#pragma warning disable CS0108, CS0114

public class Player : MonoBehaviour {
	[SerializeField] private Vector2 inputVector2;

	[SerializeField] private float speed;

	private Rigidbody2D rigidbody2D;
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	private Scanner scanner;
	[SerializeField] private Hand[] hands;
	[SerializeField] private RuntimeAnimatorController[] animatorControllers;

	private GameManager gameManager;

	private void Awake() {
		rigidbody2D = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		scanner = GetComponent<Scanner>();
		hands = GetComponentsInChildren<Hand>(true);

		gameManager = GameManager.Instance;
	}

	private void OnEnable() {
		speed *= Character.Speed;

		animator.runtimeAnimatorController = animatorControllers[gameManager.PlayerId];
	}

	private void OnMove(InputValue value) {
		inputVector2 = value.Get<Vector2>();
	}

	private void FixedUpdate() {
		if (!gameManager.IsLive) return;

		// 1. 힘을 준다
		// rigid.AddForce(inputVec);

		// 2. 속도 제어
		// rigid.velocity = inputVec;

		// 3. 위치 제어
		Vector2 nextVector2 = inputVector2 * speed * Time.fixedDeltaTime;

		rigidbody2D.MovePosition(rigidbody2D.position + nextVector2);
	}

	private void LateUpdate() {
		if (!gameManager.IsLive) return;

		animator.SetFloat("Speed", inputVector2.magnitude);

		if (inputVector2.x != 0) {
			spriteRenderer.flipX = inputVector2.x < 0;
		}
	}

	private void OnCollisionStay2D(Collision2D other) {
		if (!gameManager.IsLive) return;

		gameManager.Health -= Time.deltaTime * 10;

		if (gameManager.Health < 0) {
			for (int i = 2; i < transform.childCount; i++) {
				transform.GetChild(i).gameObject.SetActive(false);
			}

			animator.SetTrigger("Dead");

			gameManager.GameOver();
		}
	}

	public Vector2 InputVector2 => inputVector2;

	public Scanner Scanner => scanner;

	public float Speed {
		get => speed;

		set => speed = value;
	}

	public Hand[] Hands {
		get => hands;

		set => hands = value;
	}
}
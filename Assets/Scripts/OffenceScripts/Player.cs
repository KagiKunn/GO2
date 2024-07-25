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

	private void Awake() {
		rigidbody2D = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		scanner = GetComponent<Scanner>();
	}

	private void OnMove(InputValue value) {
		inputVector2 = value.Get<Vector2>();
	}

	private void FixedUpdate() {
		// 1. 힘을 준다
		// rigid.AddForce(inputVec);

		// 2. 속도 제어
		// rigid.velocity = inputVec;

		// 3. 위치 제어
		Vector2 nextVector2 = inputVector2 * speed * Time.fixedDeltaTime;

		rigidbody2D.MovePosition(rigidbody2D.position + nextVector2);
	}

	private void LateUpdate() {
		animator.SetFloat("Speed", inputVector2.magnitude);

		if (inputVector2.x != 0) {
			spriteRenderer.flipX = inputVector2.x < 0;
		}
	}

	public Vector2 InputVector2 => inputVector2;

	public Scanner Scanner => scanner;

	public float Speed {
		get => speed;

		set => speed = value;
	}
}
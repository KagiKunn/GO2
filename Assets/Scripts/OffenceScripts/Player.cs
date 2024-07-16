using System;

using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
	[SerializeField]
	private Vector2 inputVec;

	[SerializeField]
	private float speed;

	private Rigidbody2D rigidbody2D;
	private SpriteRenderer spriteRenderer;
	private Animator animator;

	private void Awake() {
		rigidbody2D = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}

	private void OnMove(InputValue value) {
		inputVec = value.Get<Vector2>();
	}

	private void FixedUpdate() {
		// 1. 힘을 준다
		// rigid.AddForce(inputVec);

		// 2. 속도 제어
		// rigid.velocity = inputVec;

		// 3. 위치 제어
		Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;

		rigidbody2D.MovePosition(rigidbody2D.position + nextVec);
	}

	private void LateUpdate() {
		animator.SetFloat("Speed", inputVec.magnitude);

		if (inputVec.x != 0) {
			spriteRenderer.flipX = inputVec.x < 0;
		}
	}
}
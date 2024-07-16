using System;

using UnityEngine;

public class Player : MonoBehaviour {
	[SerializeField]
	private Vector2 inputVec;

	[SerializeField]
	private float speed;

	private Rigidbody2D rigid;

	private void Awake() {
		rigid = GetComponent<Rigidbody2D>();
	}

	private void Update() {
		inputVec.x = Input.GetAxisRaw("Horizontal");
		inputVec.y = Input.GetAxisRaw("Vertical");
	}

	private void FixedUpdate() {
		// 1. 힘을 준다
		// rigid.AddForce(inputVec);

		// 2. 속도 제어
		// rigid.velocity = inputVec;

		// 3. 위치 제어
		Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;

		rigid.MovePosition(rigid.position + nextVec);
	}
}
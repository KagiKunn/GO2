using System;

using UnityEngine;

#pragma warning disable CS0108, CS0114

public class Bullet : MonoBehaviour {
	[SerializeField] private float damage;
	[SerializeField] private int penetration;

	private Rigidbody2D rigidbody2D;

	private void Awake() {
		rigidbody2D = GetComponent<Rigidbody2D>();
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (!other.CompareTag("Enemy") || penetration == -1) return;

		penetration--;

		if (penetration == -1) {
			rigidbody2D.velocity = Vector2.zero;

			gameObject.SetActive(false);
		}
	}

	public void Initialized(float damage, int penetration, Vector3 direction) {
		this.damage = damage;
		this.penetration = penetration;

		if (penetration > -1) {
			rigidbody2D.velocity = direction * 15f;
		}
	}

	public float Damage() => damage;
	public int Penetration() => penetration;
}
using System;

using UnityEngine;

#pragma warning disable CS0108, CS0114

public class Enemy : MonoBehaviour {
	[SerializeField] private float speed;
	[SerializeField] private float health;
	[SerializeField] private float maxHealth;
	
	[SerializeField] private RuntimeAnimatorController[] runtimeAnimatorController;

	[SerializeField] private Rigidbody2D target;

	private bool isLive;

	private Rigidbody2D rigidbody2D;
	private Animator animator;
	private SpriteRenderer spriteRenderer;
	private Player player;

	private void Awake() {
		rigidbody2D = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();

		player = GameManager.Instance.Player;
	}

	private void FixedUpdate() {
		if (!isLive) return;

		Vector2 directionVector2 = target.position - rigidbody2D.position;
		Vector2 nextVector2 = directionVector2.normalized * speed * Time.fixedDeltaTime;

		rigidbody2D.MovePosition(rigidbody2D.position + nextVector2);
		rigidbody2D.velocity = Vector2.zero;
	}

	private void LateUpdate() {
		if (!isLive) return;

		spriteRenderer.flipX = target.position.x < rigidbody2D.position.x;
	}

	private void OnEnable() {
		target = player.GetComponent<Rigidbody2D>();
		isLive = true;
		health = maxHealth;
	}

	public void Initialized(SpawnData spawnData) {
		animator.runtimeAnimatorController = runtimeAnimatorController[spawnData.SpriteType];

		speed = spawnData.Speed;
		maxHealth = spawnData.Health;
		health = spawnData.Health;
	}
}
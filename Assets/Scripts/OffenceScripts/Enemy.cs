using System;
using System.Collections;

using UnityEngine;

#pragma warning disable CS0108, CS0114

public class Enemy : MonoBehaviour {
	[SerializeField] private float speed;
	[SerializeField] private float health;
	[SerializeField] private float maxHealth;

	[SerializeField] private RuntimeAnimatorController[] runtimeAnimatorControllers;

	[SerializeField] private Rigidbody2D target;

	private bool isLive;

	private Rigidbody2D rigidbody2D;
	private Collider2D collider2D;
	private Animator animator;
	private SpriteRenderer spriteRenderer;
	private WaitForFixedUpdate waitForFixedUpdate;

	private GameManager gameManager;
	private Player player;

	private Vector3 scale;

	private void Awake() {
		rigidbody2D = GetComponent<Rigidbody2D>();
		collider2D = GetComponent<Collider2D>();
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		waitForFixedUpdate = new WaitForFixedUpdate();

		gameManager = GameManager.Instance;
		player = gameManager.Player[gameManager.PlayerId];

		scale = transform.localScale;

		animator.SetFloat("RunState", 0.25f);
	}

	private void Update() {
		if (player.gameObject.name != "Dummy") return;

		player = gameManager.Player[gameManager.PlayerId];
	}

	private void FixedUpdate() {
		if (!gameManager.IsLive) return;

		if (!isLive || animator.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return;

		Vector2 directionVector2 = target.position - rigidbody2D.position;
		Vector2 nextVector2 = directionVector2.normalized * speed * Time.fixedDeltaTime;

		rigidbody2D.MovePosition(rigidbody2D.position + nextVector2);
		rigidbody2D.velocity = Vector2.zero;
	}

	private void LateUpdate() {
		if (!gameManager.IsLive) return;

		if (!isLive) return;

		if (target.position.x < rigidbody2D.position.x) {
			scale.x = 1.5f;
			scale.y = 1.5f;
			scale.z = 1.5f;
		} else if (target.position.x > rigidbody2D.position.x) {
			scale.x = -1.5f;
			scale.y = 1.5f;
			scale.z = 1.5f;
		}

		transform.localScale = scale;
	}

	private void OnEnable() {
		target = player.GetComponent<Rigidbody2D>();

		isLive = true;
		collider2D.enabled = true;
		rigidbody2D.simulated = true;
		spriteRenderer.sortingOrder = 2;
		animator.SetBool("Die", false);
		animator.SetFloat("RunState", 0.25f);

		health = maxHealth;
	}

	public void Initialized(SpawnData spawnData) {
		speed = spawnData.Speed + ((PlayerLocalManager.Instance.lStage - 1) * 0.1f);
		maxHealth = spawnData.Health + ((PlayerLocalManager.Instance.lStage - 1) * 5);
		health = maxHealth;

		CustomLogger.Log(speed);
		CustomLogger.Log(maxHealth);
		CustomLogger.Log(health);
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (!other.CompareTag("Bullet") || !isLive) return;

		health -= other.GetComponent<Bullet>().Damage();

		StartCoroutine(KnockBack());
		// StartCoroutine("KnockBack");

		if (health > 0) {
			animator.SetFloat("RunState", 1);

			StartCoroutine(ResetRunState());

			AudioManager.Instance.PlaySfx(AudioManager.Sfx.Hit);
		} else {
			isLive = false;
			collider2D.enabled = false;
			rigidbody2D.simulated = false;
			spriteRenderer.sortingOrder = 1;
			animator.SetBool("Die", true);

			if (gameManager.IsLive) {
				gameManager.Kill++;
				gameManager.GetExp();

				AudioManager.Instance.PlaySfx(AudioManager.Sfx.Dead);
			}
		}
	}

	IEnumerator ResetRunState() {
		yield return new WaitForSeconds(1.5f);

		animator.SetFloat("RunState", 0.25f);
	}

	IEnumerator KnockBack() {
		// yield return null; // 1프레임 쉬기

		// yield return new WaitForSeconds(2f); // 2초 쉬기

		yield return waitForFixedUpdate; // 다음 하나의 무리 프레임 딜레이

		Vector3 playerPosition = player.transform.position;
		Vector3 directionVector3 = transform.parent.position - playerPosition;

		rigidbody2D.AddForce(directionVector3.normalized * 3, ForceMode2D.Impulse);
	}

	private void Dead() {
		transform.parent.gameObject.SetActive(false);
	}
}
using System;

using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

#pragma warning disable CS0414

public class EnemyMovement : MonoBehaviour {
	[SerializeField] private float health = 10f;
	[SerializeField] public float moveSpeed = 1.0f;
	[SerializeField] private LayerMask detectionLayerMask;
	[SerializeField] private int attackDamage = 1;
	[SerializeField] private float attackSpeed = 1f;
	[SerializeField] public float runState = 0.25f;
	[SerializeField] private float attackState = 1f;
	[SerializeField] private float normalState = 0f;
	[SerializeField] private float skillState = 0f;
	[SerializeField] private Vector2 boxSize = new Vector2(2, 0.1f);
	public GameObject projectilePrefab;
	private Rigidbody2D rigid2d;
	private Animator animator;
	public Vector3 movementdirection;
	private CastleWall castleWall;
	private Collider2D hit;
	private bool isChangingBrightness = false;
	public bool isKnockedBack = false;
	public float percent = 0f;
	private bool deadJudge = true;
	public bool isBoss; //보스 여부 확인
	private GameObject horseRoot;
	public NoticeUI stageEndNotice;
	public StageEndUI stageEndUI;

	// 이벤트 선언
	public static event Action OnBossDie;

	private bool isBossDied = false;

	private void Awake() {
		// HorseRoot 오브젝트 찾기
		Transform horseRootTransform = transform.Find("HorseRoot");

		if (horseRootTransform != null) {
			horseRoot = horseRootTransform.gameObject;
		}

		rigid2d = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		animator.speed = attackSpeed;
		animator.SetFloat("SkillState", skillState);
		animator.SetFloat("NormalState", normalState);
		movementdirection = Vector3.left;

		stageEndNotice = FindFirstObjectByType<NoticeUI>();
	}

	private void Update() {
		if (!isKnockedBack) {
			if (CollisionCheck()) {
				EnemyAttack();
			} else {
				EnemyMove();
			}
		} else {
			EnemyNockout();
		}

		// 이동 방향에 따라 속도 적용
		rigid2d.velocity = movementdirection * (moveSpeed * Time.timeScale);
	}

	private bool CollisionCheck() {
		Vector2 boxCenter = (Vector2)transform.position + new Vector2(-boxSize.x / 2, 0);
		hit = Physics2D.OverlapBox(boxCenter, boxSize, 0, detectionLayerMask);

		if (hit != null && hit.name == "CastleWall") {
			castleWall = hit.GetComponent<CastleWall>();

			return true;
		}

		return false;
	}

	private void EnemyMove() {
		movementdirection = Vector3.left;
		animator.SetFloat("RunState", runState);
		animator.ResetTrigger("Attack");
	}

	private void EnemyAttack() {
		movementdirection = Vector3.zero;
		animator.SetFloat("AttackState", attackState);
		animator.SetTrigger("Attack");
	}

	private void EnemyNockout() {
		animator.ResetTrigger("Attack");
		movementdirection = Vector3.zero;
		animator.SetFloat("RunState", 1f);
	}

	void AdditionalDamage(float percent) {
		this.percent = percent;
	}

	public void isAttack() {
		if (castleWall != null) {
			if (normalState == 1f || skillState == 1f || normalState == 0.25f || skillState == 0.25f || skillState == 0.75f || normalState == 0.5f) {
				CollisionAttack();
			} else {
				castleWall.TakeDamage(attackDamage);
			}
		}
	}

	public void CollisionAttack() {
		if (castleWall != null) {
			Vector3 spawnPosition = transform.position + new Vector3(0, GetComponent<Collider2D>().bounds.size.y, 0);
			GameObject projectileInstance = Instantiate(projectilePrefab, spawnPosition, Quaternion.Euler(0, 180, 0));
			EnemyProjectile projectile = projectileInstance.GetComponent<EnemyProjectile>();

			if (projectile != null) {
				projectile.Initialize(Vector3.left, attackDamage);
			} else {
				castleWall.TakeDamage(attackDamage);
			}
		}
	}

	public void TakeDamage(float damage) {
		health -= damage * (1 + (percent / 100));

		// 코루틴이 실행 중이지 않을 때만 호출
		if (!isChangingBrightness) {
			StartCoroutine(ChangeBrightnessTemporarily(0.1f, 0.6f)); // 예: 명도를 50%로 줄임
		}

		if (health <= 0 && deadJudge) {
			Die();
		}
	}

	private IEnumerator ChangeBrightnessTemporarily(float duration, float brightnessMultiplier) {
		isChangingBrightness = true; // 코루틴이 실행 중임을 표시

		Transform parent = transform;
		Dictionary<Transform, Color> originalColors = new Dictionary<Transform, Color>();

		// 부모 오브젝트와 자식 오브젝트의 원래 색상을 저장하고 명도를 변경
		yield return StoreAndChangeBrightnessRecursively(parent, brightnessMultiplier, originalColors);

		// 지정된 시간 동안 대기
		yield return new WaitForSeconds(duration);

		// 원래 색상으로 복원
		yield return RestoreOriginalColors(originalColors);

		isChangingBrightness = false; // 코루틴 실행 종료 표시
	}

	private IEnumerator StoreAndChangeBrightnessRecursively(Transform parent, float brightnessMultiplier, Dictionary<Transform, Color> originalColors) {
		Queue<Transform> queue = new Queue<Transform>();
		queue.Enqueue(parent);

		while (queue.Count > 0) {
			Transform current = queue.Dequeue();
			SpriteRenderer spriteRenderer = current.GetComponent<SpriteRenderer>();

			if (spriteRenderer != null) {
				originalColors[current] = spriteRenderer.color;
				spriteRenderer.color = ChangeBrightness(spriteRenderer.color, brightnessMultiplier);
			}

			foreach (Transform child in current) {
				queue.Enqueue(child);
			}

			// 작업을 한 프레임에 모두 처리하지 않도록 대기
			if (queue.Count % 15 == 0) {
				yield return null;
			}
		}
	}

	private Color ChangeBrightness(Color color, float multiplier) {
		float h, s, v;
		Color.RGBToHSV(color, out h, out s, out v);
		v *= multiplier;

		return Color.HSVToRGB(h, s, v);
	}

	private IEnumerator RestoreOriginalColors(Dictionary<Transform, Color> originalColors) {
		foreach (KeyValuePair<Transform, Color> entry in originalColors) {
			SpriteRenderer spriteRenderer = entry.Key.GetComponent<SpriteRenderer>();

			if (spriteRenderer != null) {
				spriteRenderer.color = entry.Value;
			}

			// 작업을 한 프레임에 모두 처리하지 않도록 대기
			if (entry.Key.GetSiblingIndex() % 15 == 0) {
				yield return null;
			}
		}
	}

	public bool IsDead() {
		return health <= 0;
	}

	private void Die() {
		// 적이 죽었을 때의 동작 (예: 오브젝트 비활성화)
		Debug.Log("Die 호출");

		// 적의 root 의 태그 출력
		CustomLogger.Log("적 Root 태그 : " + gameObject.tag);

		// 적의 태그가 EnemyBoss 일때 실행
		if (gameObject.tag == "EnemyBoss") {
			//여기에 보스가 죽었을때의 이벤트
			//ex) 다른 스크립트로 값 전송, 메서드 실행
			// find name stageManager -> 그 안에있는 메서드 실행
			// 아니면 true값을 보내서 다른 스크립트에서 받은 값이 true 일때 메서드 실행 등...
			CustomLogger.Log("보스 사망..............", "red");

			// 보스 사망 플래그 설정
			isBossDied = true;

			// 보스 사망 이벤트 호출
			OnBossDie?.Invoke();

			Time.timeScale = 0;
			CustomLogger.Log("게임이 정지되었습니다.");

			stageEndUI.ShowChangeSceneButton();
		}

		gameObject.SetActive(false);
		deadJudge = false;
	}

	private void OnDrawGizmos() {
		Vector2 boxCenter = (Vector2)transform.position + new Vector2(-boxSize.x / 2, 0);
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(boxCenter, boxSize);
	}
}
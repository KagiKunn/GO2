using System;

using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

#pragma warning disable CS0618, CS0414 // 형식 또는 멤버는 사용되지 않습니다.

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
	[SerializeField] public int stageCount;
	[SerializeField] public int weekCount;
	public bool isRight;
	public GameObject projectilePrefab;
	private Rigidbody2D rigid2d;
	private Animator animator;
	public Vector3 movementdirection;
	private CastleWall castleWall;
	private Collider2D hit;
	private bool isChangingBrightness = false;
	public bool isKnockedBack = false;
	public float percent = 0f;
	public int gold = 10;
	private bool deadJudge = true;
	public bool isBoss; //보스 여부 확인
	private GameObject horseRoot;
	public NoticeUI stageEndNotice;

	// 이벤트 선언
	public static event Action OnBossDie;

	private bool isBossDied = false;

	private void Awake() {
		// 여기에 스테이지당 증가될 값 세팅
		// stageCount 가져오기
		// ex) health = health + health/(stage*10) stage(1,2,3,4,5)
		// 다른 속성 공격속도, 이동속도, 사거리등 해도되고 안해도 되고

		if (StageC.Instance == null) return;

		stageCount = PlayerLocalManager.Instance.lStage;
		CustomLogger.Log("무브먼트의 스테이지 카운트:"+stageCount, "black");
		
		// 기본 체력 값
		float baseHealth = health;
		// 20%씩 체력 증가 
		float plusHealth = (baseHealth * 0.2f * (stageCount));
		health = baseHealth + plusHealth;
		CustomLogger.Log("무브먼트의 증가된 plushealth값 :" + plusHealth, "black");
		
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
		if (!IsDead()) {
			rigid2d.velocity = movementdirection * (moveSpeed * Time.timeScale);
		}
	}

	private bool CollisionCheck() {
		Vector2 boxCenter;

		if (isRight) {
			boxCenter = (Vector2)transform.position + new Vector2(-boxSize.x / 2, 0);
		} else {
			boxCenter = (Vector2)transform.position + new Vector2(boxSize.x / 2, 0);
		}

		hit = Physics2D.OverlapBox(boxCenter, boxSize, 0, detectionLayerMask);

		if (hit != null && (hit.CompareTag("RightWall") || hit.CompareTag("LeftWall"))) {
			castleWall = hit.GetComponent<CastleWall>();

			return true;
		}

		return false;
	}

	private void EnemyMove() {
		if (isRight) {
			movementdirection = Vector3.left;
		} else {
			movementdirection = Vector3.right;
		}

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

				if (attackState == 0f && normalState == 0f) {
					TakeDamage(health);
				}
			}
		}
	}

	public void CollisionAttack() {
		if (castleWall != null) {
			Vector3 spawnPosition = transform.position + new Vector3(0, GetComponent<Collider2D>().bounds.size.y, 0);
			GameObject projectileInstance = Instantiate(projectilePrefab, spawnPosition, Quaternion.Euler(0, 180, 0), transform);
			EnemyProjectile projectile = projectileInstance.GetComponent<EnemyProjectile>();

			if (projectile != null) {
				if (isRight) {
					projectile.Initialize(Vector3.left, attackDamage, isRight);
				} else {
					projectile.Initialize(Vector3.right, attackDamage, isRight);
				}
			} else {
				castleWall.TakeDamage(attackDamage);
			}
		}
	}

	public void TakeDamage(float damage) {
		health -= damage * (1 + (percent / 100));

		// 코루틴이 실행 중이지 않을 때만 호출
		if (!isChangingBrightness && deadJudge) {
			StartCoroutine(ChangeBrightnessTemporarily(0.1f, 0.6f)); // 예: 명도를 50%로 줄임
		}

		if (health <= 0 && deadJudge) {
			movementdirection = Vector3.zero;
			rigid2d.velocity = movementdirection * (moveSpeed * Time.timeScale);
			animator.SetTrigger("Die");
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

	private IEnumerator StoreAndChangeBrightnessRecursively(Transform parent, float brightnessMultiplier,
	                                                        Dictionary<Transform, Color> originalColors) {
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
			if (entry.Key != null && entry.Key.gameObject != null)
			{
				SpriteRenderer spriteRenderer = entry.Key.GetComponent<SpriteRenderer>();

				if (spriteRenderer != null)
				{
					spriteRenderer.color = entry.Value;
				}

				// 작업을 한 프레임에 모두 처리하지 않도록 대기
				if (entry.Key.GetSiblingIndex() % 15 == 0)
				{
					yield return null;
				}
			}
		}
	}

	public bool IsDead() {
		
		
		return health <= 0;
	}

	private void Die() {
		// 적이 죽었을 때의 동작 (예: 오브젝트 비활성화)
		if (SceneManager.GetActiveScene().name == "Defense")
		{
			if (GameObject.Find("InitSetting").GetComponent<DefenseInit>() == null) return;
			DefenseInit defenseInit = GameObject.Find("InitSetting").GetComponent<DefenseInit>();
			int crntgold;

			if (defenseInit.extraGold1 == 0)
			{
				defenseInit.currentGold += gold;
			} else {
				crntgold = gold + defenseInit.extraGold1 / gold;
				defenseInit.currentGold += crntgold;
			}
			EnemySpawner enemySpawner = GameObject.Find("Spawner").GetComponent<EnemySpawner>();
			enemySpawner.enemyDieCount++;
			enemySpawner.totalEnemyDieCount++;
			CustomLogger.Log("적 사망 카운트 :" + enemySpawner.enemyDieCount, "white");

			// 적의 태그가 EnemyBoss 일때 실행
			if (gameObject.CompareTag("EnemyBoss")) {
				//여기에 보스가 죽었을때의 이벤트
				CustomLogger.Log("보스 사망..............", "red");
				// 보스 사망 플래그 설정
				isBossDied = true;
				// 보스 사망 이벤트 호출
				OnBossDie?.Invoke();

				defenseInit.Soul ++;
				
				StageC stageC = FindObjectOfType<StageC>();
				stageC.ShowStageClearUI();
			}
		
			gameObject.SetActive(false);
			deadJudge = false;
		}
		else
		{
			transform.parent.gameObject.SetActive(false);
		}
		
	}

	private void OnDrawGizmos() {
		Vector2 boxCenter;

		if (isRight) {
			boxCenter = (Vector2)transform.position + new Vector2(-boxSize.x / 2, 0);
		} else {
			boxCenter = (Vector2)transform.position + new Vector2(boxSize.x / 2, 0);
		}

		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(boxCenter, boxSize);
	}

	public bool IsRight {
		get => isRight;

		set => isRight = value;
	}
}
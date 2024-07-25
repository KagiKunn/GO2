using UnityEngine;

#pragma warning disable CS0414

public class EnemyMovement : MonoBehaviour {
	[SerializeField] private int health = 10;

	[SerializeField]
	private float moveSpeed = 1.0f;

	[SerializeField]
	private LayerMask detectionLayerMask;

	[SerializeField] private int attackDamage = 1;

	[SerializeField] private float attackSpeed = 1f;

	//idle 0 , run 0.5, stun 1
	[SerializeField] private float runState = 1f;

	//skill 0, normal 1
	[SerializeField] private float attackState = 1f;

	//normal 0, bow 0.25 magic, 0.5 gun 0.75, crossbow 1
	[SerializeField] private float normalState = 0f;

	//normal 0, bow 0.5, magic 1
	[SerializeField] private float skillState = 0f;

	[SerializeField]
	private Vector2 boxSize = new Vector2(2, 0.1f);

	public GameObject projectilePrefab;
	private Rigidbody2D rigid2d;
	private Animator animator;
	private Vector3 movementdirection;
	private CastleWall castleWall;
	private Collider2D hit;

	private void Awake() {
		// pos.position = new Vector2(0, 0);
		rigid2d = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		animator.speed = attackSpeed;
		animator.SetFloat("SkillState", skillState);
		animator.SetFloat("NormalState", normalState);
		movementdirection = Vector3.left;
	}

	private void Update() {
		if (CollisionCheck()) {
			EnemyAttack();
		} else {
			EnemyMove();
		}

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
		animator.SetFloat("RunState", 0.5f);
		animator.ResetTrigger("Attack");
	}

	private void EnemyAttack() {
		movementdirection = Vector3.zero;
		animator.SetFloat("AttackState", attackState);
		animator.SetTrigger("Attack");
	}

	public void isAttack() {
		if (castleWall != null) {
			if (normalState == 1f || skillState == 1f || normalState == 0.25f || skillState == 0.25f)
			{
				CollisionAttack();
			}
			else
			{
				castleWall.TakeDamage(attackDamage);
			}
		}
	}

	public void CollisionAttack()
	{
		if (castleWall != null)
		{
			GameObject projectileInstance = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
			EnemyProjectile projectile = projectileInstance.GetComponent<EnemyProjectile>();
			if (projectile != null)
			{
				projectile.Initialize(castleWall.transform, attackDamage);
			}
			else
			{
				castleWall.TakeDamage(attackDamage);
			}
		}
	}
	public void TakeDamage(int damage) {
		health -= damage;
		Debug.Log(gameObject + "유닛의 체력: " + health);

		if (health <= 0) {
			Die();
		}
	}

	public bool IsDead() {
		return health <= 0;
	}

	private void Die() {
		// 적이 죽었을 때의 동작 (예: 오브젝트 비활성화)
		gameObject.SetActive(false);
		CustomLogger.Log(gameObject + "유닛이 파괴되었습니다!", "blue");
	}

	private void OnDrawGizmos() {
		Vector2 boxCenter = (Vector2)transform.position + new Vector2(-boxSize.x / 2, 0);
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(boxCenter, boxSize);
	}
}
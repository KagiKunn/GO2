using System;

using UnityEngine;

#pragma warning disable CS0219 // 변수가 할당되었지만 해당 값이 사용되지 않았습니다.
#pragma warning disable CS0414

public class AllyScan : MonoBehaviour {
	[SerializeField] private float scanRange = 10f; // 반지름
	[SerializeField] private LayerMask targetLayer;
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
	private Animator animator;
	private Transform nearestTarget;
	private RaycastHit2D[] targets;
	private static readonly int AttackAnimationHash = Animator.StringToHash("2_Attack_Bow");

	private void Awake() {
		animator = GetComponent<Animator>();
		animator.speed = attackSpeed;
		animator.SetFloat("SkillState", skillState);
		animator.SetFloat("NormalState", normalState);
	}

	private void FixedUpdate() {
		targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);

		nearestTarget = GetNearest();
	}

	Transform GetNearest() {
		Transform result = null;

		float diff = 100;

		foreach (RaycastHit2D target in targets) {
			//CustomLogger.Log("Detected");
			animator.SetFloat("AttackState", attackState);
			animator.SetTrigger("Attack");
			Vector3 myPosition = transform.position;
			Vector3 targetPosition = target.transform.position;

			float currentDiff = Vector3.Distance(myPosition, targetPosition);

			if (currentDiff < diff) {
				diff = currentDiff;

				result = target.transform;
			}
		}

		return result;
	}

	public Transform NearestTarget => nearestTarget;

	private void allyAttack() {
		animator.SetFloat("AttackState", attackState);
		animator.SetTrigger("Attack");
	}

	public void isAttack() {
		//여기에 데미지 로직
		// if (castleWall != null)
		// {
		//     castleWall.TakeDamage(attackDamage);
		// }
	}

	private void OnDrawGizmos() {
		Vector2 sphereCenter = (Vector2)transform.position;
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(sphereCenter, scanRange);
	}
}
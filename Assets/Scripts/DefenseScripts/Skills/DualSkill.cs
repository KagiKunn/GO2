using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.

public class DualSkill : HeroSkill {
	public float nockBackRang;
	private float rightMax = 200f;
	private float leftMax = -100f;
	public float damage;
	public float duration;
	private List<GameObject> enemyObjects;
	public bool dualActive;

	private void OnEnable() {
		dualActive = true;
	}

	public override void HeroSkillStart() {
		if (dualActive) {
			dualActive = false;
			base.HeroSkillStart();
			StartCoroutine(DualCooldown(cooldown)); // 쿨다운 코루틴을 여기서 시작
		}
	}

	protected override void OnSkillImageComplete() {
		base.OnSkillImageComplete();
		DualSkillEffect();
	}

	private void DualSkillEffect() {
		int enemyLayer = LayerMask.NameToLayer("Enemy");
		GameObject[] enemyObj = FindObjectsOfType<GameObject>();

		if (enemyObj != null) {
			enemyObjects = new List<GameObject>();

			foreach (var obj in enemyObj) {
				if (obj.layer == enemyLayer) {
					enemyObjects.Add(obj);
				}
			}

			// 모든 적에게 넉백 및 기절 효과를 적용
			foreach (var enemy in enemyObjects) {
				EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();

				if (enemyMovement != null) {
					StartCoroutine(NockBack(enemyMovement, damage));
					StartCoroutine(Slow(enemyMovement, duration));
				}
			}
		}
	}

	private IEnumerator DualCooldown(float cool) {
		float remainingTime = cool;

		while (remainingTime > 0) {
			skillPanelManager.UpdateSkillButtonCooldown(skillButton, remainingTime);

			yield return new WaitForSeconds(1f);

			remainingTime -= 1f;
		}

		dualActive = true;
		CustomLogger.Log("Dual Skill Ready", "white");
		skillPanelManager.UpdateSkillButtonCooldown(skillButton, 0);
	}

	public IEnumerator Slow(EnemyMovement target, float duration) {
		float originalSpeed = target.moveSpeed;
		float debuffSpeed = originalSpeed * 0.5f;
		target.moveSpeed = debuffSpeed;

		yield return new WaitForSeconds(duration);

		target.moveSpeed = originalSpeed;
	}

	public IEnumerator NockBack(EnemyMovement target, float damage) {
		// 넉백 거리 계산
		Vector3 targetKnockback = target.transform.position;
		Vector3 direction = target.movementdirection;
		if (target.isRight)
		{
			targetKnockback.x = target.transform.position.x + nockBackRang;
			if (targetKnockback.x > rightMax) {
				targetKnockback.x = rightMax;
			}
			
		}
		else
		{
			targetKnockback.x = target.transform.position.x - nockBackRang;
			if (targetKnockback.x < leftMax) {
				targetKnockback.x = leftMax;
			}
		}
		// 적의 위치를 넉백 거리만큼 이동
		target.transform.position = targetKnockback;

		// 데미지 적용
		target.TakeDamage(damage);

		// 이동 상태를 멈추도록 플래그 설정 및 기절 상태 설정
		target.isKnockedBack = true;
		target.movementdirection = Vector3.zero;
		target.runState = 1f;

		yield return new WaitForSeconds(1f);

		// 이동 상태 재개
		target.isKnockedBack = false;
		target.movementdirection = direction;
		target.runState = 0.25f;

		yield return null;
	}
}
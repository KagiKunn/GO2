using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "DamageEffects/CannonEffect")]
public class CannonEffect : DamageEffect
{
    private readonly Dictionary<EnemyMovement, Coroutine> activeDamageCoroutines = new Dictionary<EnemyMovement, Coroutine>();

    public override void ApplyEffect(EnemyMovement enemy, Transform projectile, int damage, float aoe)
    {
        Collider2D[] targets = AreaScan(projectile, aoe);
        if (targets != null)
        {
            CannonAttack(targets, damage);
        }
    }

    private void CannonAttack(Collider2D[] hitColliders, int damage)
    {
        foreach (Collider2D hitCollider in hitColliders)
        {
            EnemyMovement target = hitCollider.GetComponent<EnemyMovement>();
            if (target != null)
            {
                if (activeDamageCoroutines.ContainsKey(target))
                {
                    target.StopCoroutine(activeDamageCoroutines[target]);
                    activeDamageCoroutines.Remove(target);
                }

                Coroutine damageCoroutine = ApplyContinuousDamage(target, activeDamageCoroutines, Color.gray, CannonDamage(target, damage));
                activeDamageCoroutines[target] = damageCoroutine;
            } 
        }
    }

    public IEnumerator CannonDamage(EnemyMovement target, int damage)
    {
        // 데미지 적용
        target.TakeDamage(damage);
    
        target.isKnockedBack = true; // 이동 상태를 멈추도록 플래그 설정
        target.movementdirection = Vector3.zero;
        target.runState = 1f;
    
        yield return new WaitForSeconds(1f); // 1초 대기

        target.isKnockedBack = false; // 이동 상태 재개
        target.movementdirection = Vector3.left;
        target.runState = 0.25f;

        // 코루틴이 끝난 후 색상 복원이 이루어지도록 합니다.
        yield return null;
    }
}

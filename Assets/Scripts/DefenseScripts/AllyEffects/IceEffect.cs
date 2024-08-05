using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DamageEffects/IceEffect")]
public class IceEffect : DamageEffect
{
    private readonly Dictionary<EnemyMovement, Coroutine> activeDamageCoroutines = new Dictionary<EnemyMovement, Coroutine>();

    public override void ApplyEffect(EnemyMovement enemy, Transform projectile, int damage, float aoe)
    {
        Collider2D[] targets = AreaScan(projectile, aoe);
        if (targets != null)
        {
            IceAttack(targets, damage);
        }
        CustomLogger.Log("Ice Damage", "blue");
    }

    private void IceAttack(Collider2D[] hitColliders, int damage)
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

                Coroutine damageCoroutine = ApplyContinuousDamage(target, activeDamageCoroutines, Color.blue, IceDamage(target, damage));
                activeDamageCoroutines[target] = damageCoroutine;
            }
        }
    }

    public IEnumerator IceDamage(EnemyMovement target, int damage)
    {
        target.TakeDamage(damage);
        float duration = 5f; // 지속 시간 5초
        float elapsedTime = 0f;
        float originalSpeed = target.moveSpeed; // 원래 이동 속도를 저장합니다.
        target.moveSpeed -= originalSpeed * 0.2f; // 이동 속도를 20% 감소시킵니다.
        while (elapsedTime < duration)
        {
            CustomLogger.Log("Slowed " + target.name + " with ice effect", "blue");
            elapsedTime += 1f;
            yield return new WaitForSeconds(1f); // 1초마다 반복
        }

        target.moveSpeed = originalSpeed; // 원래 이동 속도로 되돌립니다.
    }
}
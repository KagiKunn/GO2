using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DamageEffects/LightningEffect")]
public class LightningEffect : DamageEffect
{
    private readonly Dictionary<EnemyMovement, Coroutine> activeDamageCoroutines = new Dictionary<EnemyMovement, Coroutine>();

    public override void ApplyEffect(EnemyMovement enemy, Transform projectile, int damage, float aoe)
    {
        Collider2D[] targets = AreaScan(projectile, aoe);
        if (targets != null)
        {
            LightningAttack(targets, damage);
        }
        CustomLogger.Log("Ice Damage", "blue");
    }

    private void LightningAttack(Collider2D[] hitColliders, int damage)
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

                Coroutine damageCoroutine = ApplyContinuousDamage(target, activeDamageCoroutines, Color.yellow, LightningDamage(target, damage));
                activeDamageCoroutines[target] = damageCoroutine;
            }
        }
    }

    public IEnumerator LightningDamage(EnemyMovement target, int damage)
    {
        target.TakeDamage(damage);
        float duration = 5f; // 지속 시간 5초
        float elapsedTime = 0f;
        float originalPercent = target.percent; // 원래 받는피해 저장
        target.percent = 10f; // 받는피해 10% 증가시킵니다.
        while (elapsedTime < duration)
        {
            CustomLogger.Log("Additional" + target.name + " with lightning effect", "purlple");
            elapsedTime += 1f;
            yield return new WaitForSeconds(1f); // 1초마다 반복
        }

        target.percent = originalPercent; // 원래 이동 속도로 되돌립니다.
    }
}
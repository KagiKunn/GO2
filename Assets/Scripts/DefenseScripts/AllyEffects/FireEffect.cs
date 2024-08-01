using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "DamageEffects/FireEffect")]
public class FireEffect : DamageEffect
{

    private readonly Dictionary<EnemyMovement, Coroutine> activeDamageCoroutines = new Dictionary<EnemyMovement, Coroutine>();

    public override void ApplyEffect(EnemyMovement enemy, Transform projectile, int damage, float aoe)
    {
        Collider2D[] targets = AreaScan(projectile, aoe);
        if (targets != null)
        {
            FireAttack(targets, damage);
        }
        CustomLogger.Log("Fire Damage", "red");
        // 추가적인 화염 효과 로직 (예: 화상 데미지 추가)
    }

    private void FireAttack(Collider2D[] hitColliders, int damage)
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

                Coroutine damageCoroutine = ApplyContinuousDamage(target, activeDamageCoroutines, Color.red, FireDamage(target,damage));
                activeDamageCoroutines[target] = damageCoroutine;
            }
        }
    }
    public IEnumerator FireDamage(EnemyMovement target, int damage)
    {
        float duration = 5f; // 지속 시간 5초
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            target.TakeDamage(damage);
            CustomLogger.Log("Damaged " + target.name + " with fire effect", "red");
            elapsedTime += 1f;
            yield return new WaitForSeconds(1f); // 1초마다 반복
        }
    }
}
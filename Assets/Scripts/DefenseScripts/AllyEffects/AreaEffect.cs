using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DamageEffects/AreaEffect")]
public class AreaEffect : DamageEffect
{
    private readonly Dictionary<EnemyMovement, Coroutine> activeDamageCoroutines = new Dictionary<EnemyMovement, Coroutine>();

    public override void ApplyEffect(EnemyMovement enemy, Transform projectile, int damage, float aoe)
    {
        Collider2D[] targets = AreaScan(projectile, aoe);
        if (targets != null)
        {
            AreaAttack(targets, damage);
        }
        CustomLogger.Log("Ice Damage", "blue");
    }

    private void AreaAttack(Collider2D[] hitColliders, int damage)
    {
        foreach (Collider2D hitCollider in hitColliders)
        {
            EnemyMovement target = hitCollider.GetComponent<EnemyMovement>();
            if (target != null)
            {
                AreaDamage(target, damage);
            }
        }
    }

    void AreaDamage(EnemyMovement target, int damage)
    {
        target.TakeDamage(damage);
    }
}
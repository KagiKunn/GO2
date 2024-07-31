using UnityEngine;

[CreateAssetMenu(menuName = "DamageEffects/NormalEffect")]
public class NormalEffect : DamageEffect
{
    public override void ApplyEffect(EnemyMovement enemy,Transform projectile, int damage)
    {
        enemy.TakeDamage(damage);
        CustomLogger.Log("Normal Damage","white");
    }
}
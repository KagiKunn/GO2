using UnityEngine;

[CreateAssetMenu(menuName = "DamageEffects/LightningEffect")]
public class LightningEffect : DamageEffect
{
    public override void ApplyEffect(EnemyMovement enemy,Transform projectile, int damage)
    {
        enemy.TakeDamage(damage);
        CustomLogger.Log("Lightning Damage","yellow");
    }
}
using UnityEngine;

[CreateAssetMenu(menuName = "DamageEffects/IceEffect")]
public class IceEffect : DamageEffect
{
    public override void ApplyEffect(EnemyMovement enemy, Transform projectile, int damage, float aoe)
    {
        enemy.TakeDamage(damage);
        CustomLogger.Log("Ice Damage","blue");
        // 추가적인 얼음 효과 로직 (예: 적 속도 감소)
    }
}
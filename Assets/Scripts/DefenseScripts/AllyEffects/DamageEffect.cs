using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class DamageEffect : ScriptableObject
{
    public abstract void ApplyEffect(EnemyMovement enemy,Transform projectile, int damage);
}

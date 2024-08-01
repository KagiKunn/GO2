using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class DamageEffect : ScriptableObject
{
    public LayerMask detectionLayer;

    public abstract void ApplyEffect(EnemyMovement enemy, Transform projectile, int damage, float aoe);

    protected Collider2D[] AreaScan(Transform projectile, float detectionRadius)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(projectile.position, detectionRadius, detectionLayer);
        if (hitColliders != null && hitColliders.Length > 0)
        {
            return hitColliders;
        }
        else
        {
            CustomLogger.LogWarning("<i><b><color=#FFFF00FF>[FireEffect::AreaAttack()] No enemy in area</color></b></i>");
            return null;
        }
    }

    protected void StoreAndChangeColor(Transform parent, Color newColor, Dictionary<Transform, Color> originalColors)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(parent);

        while (queue.Count > 0)
        {
            Transform current = queue.Dequeue();
            SpriteRenderer spriteRenderer = current.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalColors[current] = spriteRenderer.color;
                spriteRenderer.color = newColor;
            }

            foreach (Transform child in current)
            {
                queue.Enqueue(child);
            }
        }
    }

    protected void RestoreOriginalColors(Dictionary<Transform, Color> originalColors)
    {
        foreach (KeyValuePair<Transform, Color> entry in originalColors)
        {
            SpriteRenderer spriteRenderer = entry.Key.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = entry.Value;
            }
        }
    }

    protected Coroutine ApplyContinuousDamage(EnemyMovement target, int damage, Dictionary<EnemyMovement, Coroutine> activeDamageCoroutines, Color newColor)
    {
        return target.StartCoroutine(ApplyContinuousDamageCoroutine(target, damage, activeDamageCoroutines, newColor));
    }

    private IEnumerator ApplyContinuousDamageCoroutine(EnemyMovement target, int damage, Dictionary<EnemyMovement, Coroutine> activeDamageCoroutines, Color newColor)
    {
        // 색상 변경 시작
        Dictionary<Transform, Color> originalColors = new Dictionary<Transform, Color>();
        Transform parent = target.transform;
        StoreAndChangeColor(parent, newColor, originalColors);

        yield return target.StartCoroutine(DurationDamage(target, damage));

        // 원래 색상으로 복원
        RestoreOriginalColors(originalColors);

        activeDamageCoroutines.Remove(target);
    }

    protected IEnumerator DurationDamage(EnemyMovement target, int damage)
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

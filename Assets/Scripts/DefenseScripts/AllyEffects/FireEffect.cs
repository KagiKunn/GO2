using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "DamageEffects/FireEffect")]
public class FireEffect : DamageEffect
{
    public float detectionRadius = 5.0f;
    public LayerMask detectionLayer;

    private Dictionary<EnemyMovement, Coroutine> activeDamageCoroutines = new Dictionary<EnemyMovement, Coroutine>();

    public override void ApplyEffect(EnemyMovement enemy, Transform projectile, int damage)
    {
        AreaAttack(enemy, projectile, damage);
        enemy.TakeDamage(damage);
        CustomLogger.Log("Fire Damage", "red");
        // 추가적인 화염 효과 로직 (예: 화상 데미지 추가)
    }

    void AreaAttack(EnemyMovement enemy, Transform projectile, int damage)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(projectile.position, detectionRadius, detectionLayer);
        if (hitColliders != null && hitColliders.Length > 0)
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

                    Coroutine damageCoroutine = target.StartCoroutine(ApplyContinuousDamage(target, damage));
                    activeDamageCoroutines[target] = damageCoroutine;
                }
            }
        }
        else
        {
            CustomLogger.LogWarning("<i><b><color=#FFFF00FF>[FireEffect::AreaAttack()] No enemy in area</color></b></i>");
        }
    }

    private IEnumerator ApplyContinuousDamage(EnemyMovement target, int damage)
    {
        float duration = 5f; // 지속 시간 5초
        float elapsedTime = 0f;

        // 색상 변경 시작
        Dictionary<Transform, Color> originalColors = new Dictionary<Transform, Color>();
        Transform parent = target.transform;
        StoreAndChangeColor(parent, Color.red, originalColors);

        while (elapsedTime < duration)
        {
            target.TakeDamage(1);
            CustomLogger.Log("Damaged " + target.name + " with fire effect", "red");
            elapsedTime += 1f;
            yield return new WaitForSeconds(1f); // 1초마다 반복
        }

        // 원래 색상으로 복원
        RestoreOriginalColors(originalColors);

        activeDamageCoroutines.Remove(target);
    }

    private void StoreAndChangeColor(Transform parent, Color newColor, Dictionary<Transform, Color> originalColors)
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

    private void RestoreOriginalColors(Dictionary<Transform, Color> originalColors)
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
}

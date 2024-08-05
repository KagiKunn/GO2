using UnityEngine;

public class AllyScan : MonoBehaviour
{
    public float detectionRadius = 5.0f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float runState = 0f;
    [SerializeField] private float attackState;
    [SerializeField] private float normalState;
    [SerializeField] private float skillState;
    [SerializeField] private float aoe = 2f;
    public float movementSpeed = 10f;
    
    [SerializeField] private DamageEffect damageEffect; // damageEffect를 SerializeField로 추가
    public GameObject projectilePrefab;

    private Animator animator;
    private GameObject closestObject;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("RunState", runState);
        animator.SetFloat("SkillState", skillState);
        animator.SetFloat("NormalState", normalState);
    }

    private void Update()
    {
        if (closestObject == null)
        {
            AllyIdle();
            FindClosestObject();
        }
        else
        {
            AllyAttack();
        }
        SetAnimationSpeed("AttackState",attackSpeed);
    }
    public void SetAnimationSpeed(string name, float speed)
    {
        // AnimatorStateInfo를 사용하여 현재 상태가 공격 상태인지 확인하고, 속도를 변경합니다.
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(name))
        {
            animator.speed = speed;
        }
        else
        {
            animator.speed = 1f;
        }
    }
    void FindClosestObject()
    {
        Vector2 point = transform.position;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(point, detectionRadius, enemyLayer);

        float closestDistance = Mathf.Infinity;
        Collider2D closestCollider = null;

        foreach (var hitCollider in hitColliders)
        {
            float distance = Vector2.Distance(point, hitCollider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCollider = hitCollider;
            }
        }

        if (closestCollider != null)
        {
            closestObject = closestCollider.gameObject;
        }
    }

    private void AllyAttack()
    {
        animator.ResetTrigger("Idle");
        animator.SetFloat("AttackState", attackState);
        animator.SetTrigger("Attack");
    }

    private void AllyIdle()
    {
        animator.ResetTrigger("Attack");
        animator.SetFloat("RunState", runState);
        animator.SetTrigger("Idle");
    }

    public void isAttack()
    {
        if (closestObject != null)
        {
            if (normalState == 1f || skillState == 1f || normalState == 0.25f || skillState == 0.25f || skillState == 0.75f || normalState == 0.5f)
            {
                CollisionAttack();
            }
            else
            {
                HitScanAttack();
            }
            AllyIdle();
        }
    }

    public void HitScanAttack()
    {
        EnemyMovement enemy = closestObject.GetComponent<EnemyMovement>();
        if (enemy != null)
        {
            damageEffect.ApplyEffect(enemy,null, attackDamage, aoe); // 데미지 효과 적용
            if (enemy.IsDead())
            {
                closestObject = null;
            }
        }
    }

    public void CollisionAttack()
    {
        if (closestObject != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, GetComponent<Collider2D>().bounds.size.y / 3, 0);
            GameObject projectileInstance = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            AllyProjectile projectile = projectileInstance.GetComponent<AllyProjectile>();
            if (projectile != null)
            {
                projectile.Initialize(closestObject.transform, attackDamage, damageEffect, aoe); // 데미지 효과 전달
            }
            else
            {
                HitScanAttack();
            }

            closestObject = null;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

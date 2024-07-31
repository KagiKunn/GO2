using UnityEngine;

public class AllyScan : MonoBehaviour
{
    public float detectionRadius = 5.0f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackSpeed = 1f;
    //idle 0 , run 0.5, stun 1
    [SerializeField] private float runState = 0f;
    //skill 0, normal 1
    [SerializeField] private float attackState = 1f;
    //normal 0, bow 0.25 magic, 0.5 gun 0.75, crossbow 1
    [SerializeField] private float normalState = 0f;
    //normal 0, bow 0.5, magic 1
    [SerializeField] private float skillState = 0f;

    [SerializeField] private DamageEffect damageEffect; // damageEffect를 SerializeField로 추가
    public GameObject projectilePrefab;

    private Animator animator;
    private GameObject closestObject;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.speed = attackSpeed;
        animator.speed = attackSpeed;
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
            damageEffect.ApplyEffect(enemy,null, attackDamage); // 데미지 효과 적용
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
                projectile.Initialize(closestObject.transform, attackDamage, damageEffect); // 데미지 효과 전달
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

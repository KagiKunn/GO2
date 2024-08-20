using System;
using UnityEngine;

public class AllyProjectile : MonoBehaviour
{
    //투사체 속도
    public float speed = 10f;
    //투사체 반경
    private float aoe;
    private int damage;
    private Transform target;
    private DamageEffect damageEffect;
    private bool isRight;
    private SpriteRenderer spriteRenderer;
    
    
    public void Initialize(Transform target, int damage, DamageEffect effect, float aoe, bool isRight)
    {
        this.target = target;
        this.damage = damage;
        this.damageEffect = effect;
        this.aoe = aoe;
        this.isRight = isRight;
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = isRight;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        EnemyMovement enemy = target.GetComponent<EnemyMovement>();
        if (enemy != null)
        {
            damageEffect.ApplyEffect(enemy, transform, damage, aoe); // 데미지 효과 적용
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == target)
        {
            HitTarget();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aoe);
    }
}
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class AllyScan : MonoBehaviour
{
    public float detectionRadius = 5.0f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] public int attackDamage = 1;
    [SerializeField] private float attackCool = 1f;
    [SerializeField] private float runState = 0f;
    [SerializeField] private float attackState;
    [SerializeField] private float normalState;
    [SerializeField] private float skillState;
    [SerializeField] private float aoe = 2f;
    public float movementSpeed = 10f;

    [SerializeField] private DamageEffect damageEffect; // damageEffect를 SerializeField로 추가
    public GameObject projectilePrefab;

    public AudioClip soundClip;
    private AudioSource attackAudioSource;
    public AudioMixerGroup sfxMixerGroup;
    private GameObject currentEffect; // 현재 효과 오브젝트를 저장할 필드
    private Animator animator;
    private GameObject closestObject;
    private Coroutine attackSpeedCoroutine;
    private bool isRight;
    private bool isCooldown = false;

    public void Initialized(bool result)
    {
        this.isRight = result;
    }

    private void Start()
    {
        attackAudioSource = gameObject.AddComponent<AudioSource>();
        attackAudioSource.clip = soundClip;
        attackAudioSource.outputAudioMixerGroup = sfxMixerGroup;
        attackCool = 1f / attackCool;
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
        else if (!isCooldown) // 쿨타임 중이 아닐 때만 공격
        {
            AllyAttack();
            StartCoroutine(AttackCoolDown());
        }
        else
        {
            AllyIdle();
        }
    }
    
    public void PlaySound()
    {
        if (attackAudioSource != null && soundClip != null)
        {
            attackAudioSource.Play();
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
        FindClosestObject(); // 매 공격 전에 가장 가까운 적을 재탐지

        if (closestObject != null) // 탐지된 적이 있을 때만 공격 수행
        {
            animator.ResetTrigger("Idle");
            animator.SetFloat("AttackState", attackState);
            animator.SetTrigger("Attack");
        }
        else
        {
            AllyIdle(); // 적이 없으면 Idle 상태로 전환
        }
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

            closestObject = null;
            AllyIdle();
        }
    }

    private IEnumerator AttackCoolDown()
    {
        isCooldown = true; // 쿨타임 시작
        yield return new WaitForSeconds(attackCool);
        isCooldown = false; // 쿨타임 종료
    }
    public void HitScanAttack()
    {
        EnemyMovement enemy = closestObject.GetComponent<EnemyMovement>();
        if (enemy != null)
        {
            PlaySound();
            damageEffect.ApplyEffect(enemy, null, attackDamage, aoe); // 데미지 효과 적용
            if (enemy.IsDead())
            {
                closestObject = null;
            }
        }
    }

    private void CollisionAttack()
    {
        if (closestObject != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, GetComponent<Collider2D>().bounds.size.y / 3, 0);
            GameObject projectileInstance = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity,transform);
            AllyProjectile projectile = projectileInstance.GetComponent<AllyProjectile>();
            if (projectile != null)
            {
                PlaySound();
                projectile.Initialize(closestObject.transform, attackDamage, damageEffect, aoe, isRight); // 데미지 효과 전달
                
                
            }
            else
            {
                HitScanAttack();
            }

            closestObject = null;
        }
    }

    public void EnhanceAttack(float duration, GameObject effectPrefab)
    {
        // 기존의 코루틴이 실행 중이면 중지합니다.
        if (attackSpeedCoroutine != null)
        {
            StopCoroutine(attackSpeedCoroutine);
        }

        // 기존 효과 오브젝트가 있다면 삭제합니다.
        if (currentEffect != null)
        {
            Destroy(currentEffect);
        }

        // 공격력을 두 배로 증가시킵니다.
        attackDamage *= 2;
        attackCool /= 2;

        // 새로운 효과 오브젝트를 생성합니다.
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, 100);
        currentEffect = Instantiate(effectPrefab, pos, Quaternion.identity, transform);

        Canvas canvas = currentEffect.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = currentEffect.AddComponent<Canvas>();
            canvas.overrideSorting = true;
        }
        canvas.sortingOrder = 100;

        // 지정된 기간 동안 효과를 유지하는 코루틴을 시작합니다.
        attackSpeedCoroutine = StartCoroutine(ResetAttackSpeedAfterDelay(duration));
    }

    private IEnumerator ResetAttackSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        attackCool *= 2; // 공격 속도를 원래대로 되돌립니다.
        attackDamage /= 2; // 공격력을 원래대로 되돌립니다.

        // 효과 오브젝트가 있다면 삭제합니다.
        if (currentEffect != null)
        {
            Destroy(currentEffect);
            currentEffect = null;
        }
        else
        {
            CustomLogger.LogError("no effect!");
        }
        Debug.Log("Attack speed and damage reset to original values for: " + gameObject.name);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

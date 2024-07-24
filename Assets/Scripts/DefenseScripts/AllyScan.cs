using System;

using UnityEngine;

#pragma warning disable CS0219 // 변수가 할당되었지만 해당 값이 사용되지 않았습니다.

public class AllyScan : MonoBehaviour {
    [SerializeField] private float scanRange; // 반지름
    [SerializeField] private LayerMask targetLayer;

    private Animator animator;
    private Transform nearestTarget;
    private RaycastHit2D[] targets;
    private static readonly int AttackAnimationHash = Animator.StringToHash("2_Attack_Bow");
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate() {
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        
        nearestTarget = GetNearest();
    }

    Transform GetNearest() {
        Transform result = null;

        float diff = 100;

        foreach (RaycastHit2D target in targets) {
            CustomLogger.Log("Detected");
            animator.Play(AttackAnimationHash);
            Vector3 myPosition = transform.position;
            Vector3 targetPosition = target.transform.position;

            float currentDiff = Vector3.Distance(myPosition, targetPosition);

            if (currentDiff < diff) {
                diff = currentDiff;

                result = target.transform;
            }
        }

        return result;
    }

    public Transform NearestTarget => nearestTarget;
    
    private void OnDrawGizmos()
    {
        Vector2 sphereCenter = (Vector2)transform.position;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(sphereCenter, scanRange);
    }
}
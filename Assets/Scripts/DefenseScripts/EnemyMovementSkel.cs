using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovementSkel : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.0f;
    [SerializeField]
    private LayerMask detectionLayerMask;

    [SerializeField]
    private Vector2 boxSize = new Vector2(1, 2);
    private Rigidbody2D _rigid2d;
    private Animator _animator;
    private static readonly int RunAnimationHash = Animator.StringToHash("1_Run");
    private static readonly int AttackAnimationHash = Animator.StringToHash("2_Attack_Normal");
    private Vector3 movementdirection;
    private CastleWall castleWall;
    private bool isAttacking = false;
    
    private void Awake()
    {
        // pos.position = new Vector2(0, 0);
        _rigid2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        movementdirection = Vector3.left;
    }

    private void Update()
    {
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(-boxSize.x/2, 0);
        Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, 0,detectionLayerMask);
        if (hit != null && hit.name == "CastleWall")
        {
            if (!isAttacking)
            {
                isAttacking = true;
                _animator.Play(AttackAnimationHash);
                castleWall = hit.GetComponent<CastleWall>();
                if (castleWall != null)
                {
                    castleWall.TakeDamage(1);
                }
                Invoke("ResetAttack", 1.0f);
            }
            movementdirection = Vector3.zero;
        }
        else
        {
            _animator.Play(RunAnimationHash);
            movementdirection = Vector3.left;
            isAttacking = false;
        }
        _rigid2d.velocity = movementdirection * (speed * Time.timeScale);
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }
    

    private void OnDrawGizmos()
    {
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(-boxSize.x/2, 0);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
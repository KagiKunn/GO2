using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
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
    private static readonly int AttackAnimationHash = Animator.StringToHash("2_Attack_Bow");
    private Vector3 movementdirection;
    private void Awake()
    {
        // pos.position = new Vector2(0, 0);
        _rigid2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        movementdirection = Vector3.left;
    }

    private void Update()
    {
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(0, -boxSize.y / 2);
        Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, 0,detectionLayerMask);
        if (hit != null && hit.name == "CastleWall")
        {
            _animator.Play(AttackAnimationHash);
            movementdirection = Vector3.zero;
        }
        else
        {
            _animator.Play(RunAnimationHash);
        }
        _rigid2d.velocity = movementdirection * (speed * Time.timeScale);
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(-boxSize.x/2, 0);
        Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, 0,detectionLayerMask);
        if (hit != null && hit.name == "CastleWall")
            {
                _animator.Play(AttackAnimationHash);
                movementdirection = Vector3.zero;
            }
        else
        {
            _animator.Play(RunAnimationHash);
        }
            _rigid2d.velocity = movementdirection * (speed * Time.timeScale);
    }

    private void OnDrawGizmos()
    {
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(-boxSize.x / 2, 0);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}

using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.0f;
    [SerializeField]
    private LayerMask detectionLayerMask;

    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackSpeed = 1f;
    
    [SerializeField]
    private Vector2 boxSize = new Vector2(2, 0.1f);
    private Rigidbody2D rigid2d;
    private Animator animator;
    private static readonly int RunAnimationHash = Animator.StringToHash("1_Run");
    private static readonly int AttackAnimationHash = Animator.StringToHash("2_Attack_Normal");
    private Vector3 movementdirection;
    private CastleWall castleWall;
    private Collider2D hit;
    private void Awake()
    {
        // pos.position = new Vector2(0, 0);
        rigid2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.speed = attackSpeed;
        movementdirection = Vector3.left;
    }

    private void Update()
    {
        if (CollisionCheck())
        {
            EnemyAttack();
        }
        else
        {
            EnemyMove();
        }
        rigid2d.velocity = movementdirection * (speed * Time.timeScale);
    }

    private bool CollisionCheck()
    {
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(-boxSize.x/2, 0);
        hit = Physics2D.OverlapBox(boxCenter, boxSize, 0,detectionLayerMask);
        if(hit != null && hit.name == "CastleWall")
        {
            castleWall = hit.GetComponent<CastleWall>();
            return true;
        }
        return false;
    }

    private void EnemyMove()
    {
        animator.Play(RunAnimationHash);
        movementdirection = Vector3.left;
    }

    private void EnemyAttack()
    {
        animator.Play(AttackAnimationHash);
        movementdirection = Vector3.zero;
    }

    public void isAttack()
    {
        if (castleWall != null)
        {
            castleWall.TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(-boxSize.x/2, 0);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
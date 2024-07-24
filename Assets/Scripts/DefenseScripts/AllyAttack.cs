using UnityEngine;

public class AllyAttack : MonoBehaviour
{
    [SerializeField] private LayerMask detectionLayerMask;
    [SerializeField] private Vector2 boxSize = new Vector2(2, 12f);
    private Animator animator;

    
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(boxSize.x, 0);
        Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, 0,detectionLayerMask);
    }
    private void OnDrawGizmos()
    {
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(boxSize.x, 0);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}

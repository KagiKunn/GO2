using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;
    private int damage;
    private Transform target;

    public void Initialize(Transform target, int damage)
    {
        this.target = target;
        this.damage = damage;
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
        CastleWall castleWall = target.GetComponent<CastleWall>();
        if (castleWall != null)
        {
            castleWall.TakeDamage(damage);
            // if (castleWall.IsDead())
            // {
            //     CustomLogger.Log("적이 죽었습니다.");
            // }
        }
        Destroy(gameObject);
    }
}
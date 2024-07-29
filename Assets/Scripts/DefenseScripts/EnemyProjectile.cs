using UnityEngine;

public class EnemyProjectile : MonoBehaviour {
    public float speed = 10f;
    private int damage;
    private Vector3 direction;
    [SerializeField] private LayerMask targetLayerMask;

    public void Initialize(Vector3 direction, int damage) {
        this.direction = direction;
        this.damage = damage;
    }

    private void Update() {
        // 지정된 방향으로 발사체 이동
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("Projectile hit: " + collision.name); // 충돌 로그 추가
        // 지정된 레이어에 닿으면 데미지 주기
        if (((1 << collision.gameObject.layer) & targetLayerMask) != 0) {
            CastleWall targetCastleWall = collision.GetComponent<CastleWall>();
            if (targetCastleWall != null) {
                targetCastleWall.TakeDamage(damage);
            }
            Destroy(gameObject); // 투사체 파괴
        }
    }
}
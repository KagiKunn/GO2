using UnityEngine;

public class EllipseController : MonoBehaviour
{
    private GameObject ellipsePrefab;
    private LayerMask enemyLayer;
    private GameObject currentEllipse;
    private bool isEllipseActive = false;

    public void Initialize(GameObject prefab, LayerMask layer)
    {
        ellipsePrefab = prefab;
        enemyLayer = layer;
        
        // 타원형 영역 생성
        if (currentEllipse == null)
        {
            currentEllipse = Instantiate(ellipsePrefab);
            Debug.Log("Ellipse prefab instantiated");
            isEllipseActive = true;
        }
    }

    private void Update()
    {
        if (isEllipseActive)
        {
            // 마우스 위치를 월드 좌표로 변환
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                currentEllipse.transform.position = hit.point;
                Debug.Log("Ellipse position updated to: " + hit.point);
            }

            // 마우스 클릭 시 적 감지 및 처리
            if (Input.GetMouseButtonDown(0))
            {
                DetectEnemies();
            }
        }
    }

    private void DetectEnemies()
    {
        // 적을 감지하고 처리하는 로직 추가
        Collider[] enemies = Physics.OverlapSphere(currentEllipse.transform.position, 5f, enemyLayer);
        foreach (var enemy in enemies)
        {
            // 적에게 피해를 주는 로직 예시
            enemy.GetComponent<EnemyMovement>().TakeDamage(10);
        }
    }

    private void OnDisable()
    {
        if (currentEllipse != null)
        {
            Destroy(currentEllipse);
        }
    }
}
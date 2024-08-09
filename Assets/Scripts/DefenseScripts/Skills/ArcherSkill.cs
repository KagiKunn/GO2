using System.Collections;
using UnityEngine;

public class ArcherSkill : HeroSkill
{
    public GameObject ellipsePrefab; // 타원형 영역의 프리팹
    public LayerMask enemyLayer; // Enemy 레이어를 감지하기 위한 레이어 마스크
    private GameObject currentEllipse;
    public bool isEllipseActive = false;
    public float damage = 10f;
    public bool archerActive;

    private void OnEnable()
    {
        archerActive = true;
    }

    public override void HeroSkillStart()
    {
        if (archerActive)
        {
            archerActive = false;
            base.HeroSkillStart();
            StartCoroutine(ArcherCooldown(cooldown)); // 쿨다운 코루틴을 여기서 시작
        }
        else
        {
            CustomLogger.Log("Archer Skill Cooldown", "gray");
        }
    }

    protected override void OnSkillImageComplete()
    {
        base.OnSkillImageComplete();
        Debug.Log("Archer skill activated.");
        if (!isEllipseActive)
        {
            CustomLogger.Log("!isEllipseActive");
            CreateEllipse();
            isEllipseActive = true;
        }
    }

    private void Awake()
    {
        isEllipseActive = false;
    }

    private void Update()
    {
<<<<<<< HEAD
        // Debug.Log("Update called in ArcherSkill");

=======
>>>>>>> 055e7753ae471693894463c5b0489e066c0a05bd
        // 마우스 클릭 이벤트 감지
        if (Input.GetMouseButtonDown(0))
        {
            CustomLogger.Log("Mouse Button Down");
            if (isEllipseActive)
            {
                DetectEnemiesAndDestroyEllipse();
            }
            else
            {
                //HeroSkillStart(); // 마우스 클릭 시 HeroSkillStart 호출
            }
        }

        if (isEllipseActive)
        {
            FollowMouse();
        }
    }

    private IEnumerator ArcherCooldown(float cool)
    {
        float remainingTime = cool;
        while (remainingTime > 0)
        {
            skillPanelManager.UpdateSkillButtonCooldown(skillButton, remainingTime);
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }
        archerActive = true;
        CustomLogger.Log("Archer Skill Ready", "white");
        skillPanelManager.UpdateSkillButtonCooldown(skillButton, 0);
    }

    void CreateEllipse()
    {
        CustomLogger.Log("-CreateEllipse-");
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10.0f; // 카메라에서 떨어진 거리
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        currentEllipse = Instantiate(ellipsePrefab, worldPosition, Quaternion.identity);
        isEllipseActive = true;
    }

    void FollowMouse()
    {
        CustomLogger.Log("-FollowMouse-");
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10.0f; // 카메라에서 떨어진 거리
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if (currentEllipse != null)
        {
            currentEllipse.transform.position = worldPosition;
        }
    }

    void DetectEnemiesAndDestroyEllipse()
    {
        CustomLogger.Log("-DetectEnemies-");
        if (currentEllipse == null)
            return;

        // 타원형의 경계 영역을 구하기 위한 Collider2D
        Collider2D ellipseCollider = currentEllipse.GetComponent<Collider2D>();
        if (ellipseCollider != null)
        {
            // 타원형 영역 내에 있는 모든 적 감지
            Collider2D[] enemies = Physics2D.OverlapAreaAll(ellipseCollider.bounds.min, ellipseCollider.bounds.max, enemyLayer);
            if (enemies != null)
            {
                foreach (Collider2D enemy in enemies)
                {
                    Debug.Log("Enemy detected: " + enemy.gameObject.name);
                    EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
                    enemyMovement.TakeDamage(damage);
                    // 여기에 적에게 피해를 주거나 다른 작업을 수행하는 코드를 추가
                }
            }
        }

        // 타원형 영역 제거
        Destroy(currentEllipse);
        isEllipseActive = false;
    }
}

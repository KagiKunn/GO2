using UnityEngine;

[CreateAssetMenu(menuName = "HeroSkill/ArcherSkill")]
public class ArcherSkill : HeroSkill
{
    public GameObject ellipsePrefab; // 타원형 영역의 프리팹
    public LayerMask enemyLayer; // Enemy 레이어를 감지하기 위한 레이어 마스크

    public override void HeroSkillStart()
    {
        base.HeroSkillStart();
        Debug.Log("Archer skill activated.");

        // 새로운 스크립트를 실행하여 타원형 영역 제어
        GameObject ellipseController = new GameObject("EllipseController");
        EllipseController controller = ellipseController.AddComponent<EllipseController>();
        controller.Initialize(ellipsePrefab, enemyLayer);

    }
}
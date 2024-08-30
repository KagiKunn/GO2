using System.Collections;
using UnityEngine;

public class KnightSkill : HeroSkill
{
    public bool knightActive;
    private LayerMask allyLayer;
    public GameObject effect;

    private void OnEnable()
    {
        allyLayer = LayerMask.GetMask("Ally");
        knightActive = true;
    }

    public override void HeroSkillStart()
    {
        if (knightActive)
        {
            knightActive = false;
            base.HeroSkillStart();
            StartCoroutine(KnightCooldown(cooldown)); // 쿨다운 코루틴을 여기서 시작
        }
        else
        {
            CustomLogger.Log("Knight Skill Cooldown", "gray");
        }
    }

    protected override void OnSkillImageComplete()
    {
        base.OnSkillImageComplete();
        Debug.Log("Knight skill activated.");
        AllyBuff();
    }

    private IEnumerator KnightCooldown(float cool)
    {
        float remainingTime = cool;
        while (remainingTime > 0)
        {
            skillPanelManager.UpdateSkillButtonCooldown(skillButton, remainingTime);
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }
        knightActive = true;
        CustomLogger.Log("Knight Skill Ready", "white");
        skillPanelManager.UpdateSkillButtonCooldown(skillButton, 0);
    }

    void AllyBuff()
    {
        GameObject[] allyObjects = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject obj in allyObjects)
        {
            if (obj.name != "Default(Clone)")
            {
                // 첫 번째 자식 오브젝트를 가져옵니다.
                if (obj.transform.childCount > 0)
                {
                    Transform firstChild = obj.transform.GetChild(0);

                    // 첫 번째 자식 오브젝트에서 AllyScan 스크립트를 찾습니다.
                    AllyScan allyScan = firstChild.GetComponent<AllyScan>();
                    if (allyScan != null)
                    {
                        // 필드 값을 변경합니다.
                        allyScan.EnhanceAttack(10f, effect); // AllyScan 클래스로 공격 속도 증가 효과를 위임
                    }
                    else
                    {
                        CustomLogger.LogError("AllyScan is null!");
                    }
                }
            }
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;

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
            base.HeroSkillStart();
        }
        else
        {
            CustomLogger.Log("Knight Skill Cooldown", "gray");
        }
    }

    protected override void OnSkillImageComplete()
    {
        base.OnSkillImageComplete();
        if (knightActive)
        {
            Debug.Log("Knight skill activated.");
            AllyBuff();
            knightActive = false;
            CoroutineRunner.Instance.StartCoroutine(KnightCooldown(cooldown));
        }
    }

    private IEnumerator KnightCooldown(float cool)
    {
        yield return new WaitForSeconds(cool);
        knightActive = true;
        CustomLogger.Log("Knight Skill Ready", "white");
    }

    void AllyBuff()
    {
        GameObject[] allyObjects = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject obj in allyObjects)
        {
            if (obj.name != "Defualt(Clone)")
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

using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "HeroSkill/KnightSkill")]
public class KnightSkill : HeroSkill
{
    private LayerMask allyLayer;
    private void OnEnable()
    {
        // OnEnable에서 allyLayer를 초기화합니다.
        allyLayer = LayerMask.GetMask("Ally");
    }

    public override void HeroSkillStart()
    {
        if(isActive){
            base.HeroSkillStart();
            Debug.Log("Knight skill activated.");
            AllyBuff();
            isActive = false;
            CoroutineRunner.Instance.StartCoroutine(KnightCooldown(cooldown));
        }
        else
        {
            CustomLogger.Log("Knight Skill Cooldown","gray");
        }
    }

    private IEnumerator KnightCooldown(float cool)
    {
        yield return new WaitForSeconds(cooldown);
        isActive = true;
        CustomLogger.Log("Knight SKill Ready","white");
    }

    void AllyBuff()
    {
        GameObject[] allyObjects = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (GameObject obj in allyObjects)
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
                    allyScan.EnhanceAttack(10f); // AllyScan 클래스로 공격 속도 증가 효과를 위임
                }
                else
                {
                    CustomLogger.LogError("AllyScan is null!");
                }
            }
        }
    }
}
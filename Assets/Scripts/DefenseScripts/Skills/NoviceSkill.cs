using System;
using System.Collections;
using UnityEngine;

public class NoviceSkill : HeroSkill
{
    public bool noviceActive;
    public float shield;

    private void OnEnable()
    {
        noviceActive = true;
    }

    public override void HeroSkillStart()
    {
        if (noviceActive)
        {
            base.HeroSkillStart();
            
        }
        else
        {
            CustomLogger.Log("Novice Skill Cooldown", "gray");
        }
    }

    protected override void OnSkillImageComplete()
    {
        base.OnSkillImageComplete();
        if (noviceActive)
        {
            WallShield();
            Debug.Log("Novice skill activated.");
            noviceActive = false;
            CoroutineRunner.Instance.StartCoroutine(NoviceCooldown(cooldown));
        }
    }
    private IEnumerator NoviceCooldown(float cool)
    {
        yield return new WaitForSeconds(cool);
        noviceActive = true;
        CustomLogger.Log("Novice SKill Ready", "white");
    }
    void WallShield()
    {
        GameObject[] wallObjects = GameObject.FindGameObjectsWithTag("Wall");

        foreach (GameObject obj in wallObjects)
        {
            CustomLogger.Log(obj);
            CastleWall castleWall = obj.GetComponent<CastleWall>();
            if (castleWall != null)
            {
                // 필드 값을 변경합니다.
                Debug.Log("CastleWall found, applying shield.");
                castleWall.EarnShield(10f, shield);
            }
            else
            {
                CustomLogger.LogError("CastleWall component is null!");
            }
        }
    }
}

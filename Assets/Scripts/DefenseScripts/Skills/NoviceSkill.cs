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
            noviceActive = false;
            base.HeroSkillStart();
            StartCoroutine(NoviceCooldown(cooldown)); // 쿨다운 코루틴을 여기서 시작
        }
        else
        {
            CustomLogger.Log("Novice Skill Cooldown", "gray");
        }
    }

    protected override void OnSkillImageComplete()
    {
        base.OnSkillImageComplete();
        WallShield();
        Debug.Log("Novice skill activated.");
    }

    private IEnumerator NoviceCooldown(float cool)
    {
        float remainingTime = cool;
        while (remainingTime > 0)
        {
            skillPanelManager.UpdateSkillButtonCooldown(skillButton, remainingTime);
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }
        noviceActive = true;
        CustomLogger.Log("Novice Skill Ready", "white");
        skillPanelManager.UpdateSkillButtonCooldown(skillButton, 0);
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
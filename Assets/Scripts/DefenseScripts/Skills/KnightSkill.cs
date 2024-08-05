using UnityEngine;

[CreateAssetMenu(menuName = "HeroSkill/KnightSkill")]
public class KnightSkill : HeroSkill
{
    public override void HeroSkillAction()
    {
        base.HeroSkillAction();
        Debug.Log("Knight skill activated.");
    }
}
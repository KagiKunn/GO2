using UnityEngine;

[CreateAssetMenu(menuName = "HeroSkill/NoviceSkill")]
public class NoviceSkill : HeroSkill
{
    public override void HeroSkillAction()
    {
        base.HeroSkillAction();
        Debug.Log("Novice skill activated.");
    }
}
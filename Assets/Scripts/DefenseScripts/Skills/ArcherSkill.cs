using UnityEngine;

[CreateAssetMenu(menuName = "HeroSkill/ArcherSkill")]
public class ArcherSkill : HeroSkill
{
    public override void HeroSkillAction()
    {
        base.HeroSkillAction();
        Debug.Log("Archer skill activated.");
    }
}
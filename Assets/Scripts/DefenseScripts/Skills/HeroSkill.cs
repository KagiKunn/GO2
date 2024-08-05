using UnityEngine;

public abstract class HeroSkill : ScriptableObject
{
    public Sprite skillIcon;
    public virtual void HeroSkillAction()
    {
        Time.timeScale = 0.5f;
        CustomLogger.Log("Skill activated");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/ProjectileSkillCard")]
public class ProjectileSkillCard : UpgradeCard
{
    [Header("스킬 강화")]
    [SerializeField] float damageMultiplier = 1.0f;
    [SerializeField] int addProjectileCnt = 0;
    [SerializeField] float projectileSpeed = 1.0f;
    [SerializeField] ProjectileSkill skill;
    [SerializeField] string skillName;
    public override void ApplySelectedCard()
    {
        Debug.Log("스킬 강화");
        if (damageMultiplier != 1.0f)
            skill.UpgradeMultiplier(damageMultiplier);
        if (addProjectileCnt != 0)
            skill.UpgradeProjectileCount(addProjectileCnt);
        if (projectileSpeed != 1.0f)
            skill.UpgradeSpeed(projectileSpeed);
    }
    public void SetCard(ProjectileSkill s)
    {
        skill = s;
        skillName = s.name.ToString();
        skillName = skillName.Substring(0,skillName.Length - 7);
        cardTitle = string.Concat(skillName, " ", cardTitle);
        cardExplanation = string.Concat(skillName, " ", cardExplanation);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Card/RangedSkillCard")]
public class RangedSkillCard : UpgradeCard
{
    [Header("스킬 강화")]
    [SerializeField] float damageMultiplier = 1.0f;
    [SerializeField] float sizeMultiplier = 1.0f;
    [SerializeField] RangedSkill skill;
    [SerializeField] string skillName;
    public override void ApplySelectedCard()
    {
        Debug.Log("스킬 강화");
        if (damageMultiplier != 1.0f)
            skill.UpgradeMultiplier(damageMultiplier);
        if(sizeMultiplier != 1.0f)
            skill.UpgradeSize(sizeMultiplier);
    }
    public void SetCard(RangedSkill s)
    {
        skill = s;
        skillName = s.name.ToString();
        skillName = skillName.Substring(0, skillName.Length - 7);
        cardTitle = string.Concat(skillName, " ", cardTitle);
        cardExplanation = string.Concat(skillName, " ", cardExplanation);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "End Skill", menuName = "Scriptable Object/Skill Data/EndSkill")]
public class EndSkill : Skill
{
    public override void UseSkill(Entity entity, Vector2 dir)
    {
        SkillEntry enemy = entity as SkillEntry;
        enemy.isPatternEnd = true;
    }
}

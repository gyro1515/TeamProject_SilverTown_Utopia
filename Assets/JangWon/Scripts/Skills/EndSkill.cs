using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "End Skill", menuName = "Scriptable Object/Skill Data/EndSkill")]
public class EndSkill : Skill
{
    //Called Each Time when Enemy ended skill
    //Need to Add EndSkill At the end of each patterns
    public override void UseSkill(Entity entity, Vector2 dir)
    {
        //Free flag of enemy isPatternEnd
        Enemy enemy = entity as Enemy;
        enemy.isPatternEnd = true;
    }
}

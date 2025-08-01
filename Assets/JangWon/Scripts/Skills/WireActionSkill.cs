using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Wire Skill Data", menuName = "Scriptable Object/Skill Data/WireSkill")]
public class WireActionSkill : Skill
{
    [SerializeField] Wire wirePrefab;

    public override void UseSkill(Entity entity, Vector2 dir)
    {
        base.UseSkill(entity, dir);
        Wire wire = Instantiate(wirePrefab);
        wire.Init(entity, dir);
    }

}

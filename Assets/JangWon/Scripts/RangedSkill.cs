using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Ranged Skill Data", menuName = "Scriptable Object/Skill Data/RangedSkill")]
public class RangedSkill : Skill
{
    [SerializeField] HitCollider colliderPrefab;
    [SerializeField] float warningDuration = 0.0f;
    [SerializeField] float attackRemain = 0.0f;
    [SerializeField] float attackAngle = 0.0f;
    [SerializeField] public bool isFixedRotation = true;
    [SerializeField] Vector2 size = Vector2.one;

    public override void UseSkill(Entity entity, Vector2 dir)
    {
        base.UseSkill(entity, dir);
        _coolTime = warningDuration;
        HitCollider collider = Instantiate(colliderPrefab);
        collider.transform.localScale = size;
        collider.Init(shooter, 
            isLocal ? shooter.transform.localPosition + PositionCenter : PositionCenter,
            warningDuration, attackRemain, 
            isFixedRotation ? attackAngle : Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + attackAngle,
            skillDamage);
    }
}

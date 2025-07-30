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

    public override void UseSkill(Entity entity, Vector2 dir)
    {
        base.UseSkill(entity, dir);

        HitCollider collider = Instantiate(colliderPrefab);
        collider.Init(shooter, 
            isLocal ? shooter.transform.localPosition + PositionCenter : PositionCenter,
            warningDuration, attackRemain, 
            isFixedRotation ? attackAngle : Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + attackAngle,
            skillDamage);
    }
}

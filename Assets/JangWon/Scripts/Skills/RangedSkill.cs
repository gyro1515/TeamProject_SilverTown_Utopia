using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Ranged Skill Data", menuName = "Scriptable Object/Skill Data/RangedSkill")]
public class RangedSkill : Skill
{
    [SerializeField] HitCollider colliderPrefab;
    public HitCollider hitCollider;
    [SerializeField] float warningDuration = 0.0f;
    [SerializeField] float attackRemain = 0.0f;
    [SerializeField] float attackAngle = 0.0f;
    [SerializeField] public bool isFixedRotation = true;
    [SerializeField] public Vector2 size = Vector2.one;

    public override void UseSkill(Entity entity, Vector2 dir)
    {
        base.UseSkill(entity, dir);
        _coolTime = warningDuration;
        hitCollider = Instantiate(colliderPrefab);
        hitCollider.transform.localScale = size;
        hitCollider.Init(shooter, positionCenter,warningDuration, attackRemain,
            isFixedRotation ? attackAngle : Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + attackAngle,
            skillDamage);
    }
}

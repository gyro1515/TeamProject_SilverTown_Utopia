using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Ranged Skill Data", menuName = "Scriptable Object/Skill Data/RangedSkill")]
public class RangedSkill : Skill
{
    [SerializeField] Vector3 PositionCenter = Vector3.zero;
    [SerializeField] HitCollider colliderPrefab;
    [SerializeField] float warningDuration = 0.0f;
    [SerializeField] float attackRemain = 0.0f;
    [SerializeField] float attackAngle = 0.0f;
    [SerializeField] bool isLocal = true;


    public override void UseSkill(Entity entity)
    {
        base.UseSkill(entity);
        HitCollider collider = Instantiate(colliderPrefab);
        collider.Init(shooter, 
            isLocal ? shooter.transform.localPosition + PositionCenter : PositionCenter,
            warningDuration, attackRemain, attackAngle, skillDamage);
    }

}

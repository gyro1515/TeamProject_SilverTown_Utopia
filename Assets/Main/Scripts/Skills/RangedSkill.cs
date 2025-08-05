using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Ranged Skill Data", menuName = "Scriptable Object/Skill Data/RangedSkill")]
public class RangedSkill : Skill
{
    //HitCollider to copy
    [SerializeField] HitCollider colliderPrefab;
    //Copied hit collider
    public HitCollider hitCollider;
    //warning duration of hitcollider
    [SerializeField] float warningDuration = 0.0f;
    //attack remain of hitcollider
    [SerializeField] float attackRemain = 0.0f;
    //attack angle of hitcollider
    [SerializeField] float attackAngle = 0.0f;
    //Fix Rotation or not
        //if true, hitCollider won't rotate based of direction
        //if false, hitCollider will rotate based of direction -> Used for QuadCircle
    [SerializeField] public bool isFixedRotation = true;
    //HitCollider Size
    [SerializeField] public Vector2 size = Vector2.one;
    [SerializeField] bool isAnimationFixed = false;

    public override void UseSkill(Entity entity, Vector2 dir)
    {
        base.UseSkill(entity, dir);
        _coolTime = warningDuration;
        hitCollider = Instantiate(colliderPrefab);
        hitCollider.transform.localScale = size;
        hitCollider.Init(shooter, positionCenter,warningDuration, attackRemain,
            isFixedRotation ? attackAngle : Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + attackAngle,
            skillDamage, animPrefab, isAnimationFixed);
    }

    public void UpgradeSize(float sizescale)
    {
        if (sizescale < 0)
            return;
        size = size * sizescale;
    }
}

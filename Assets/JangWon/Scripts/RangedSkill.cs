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


    public override void UseSkill()
    {
        HitCollider collider = Instantiate(colliderPrefab);
        collider.Init(PositionCenter, warningDuration, attackRemain);
    }

}

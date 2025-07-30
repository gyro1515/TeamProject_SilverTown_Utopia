using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Charge Skill Data", menuName = "Scriptable Object/Skill Data/ChargeSkill")]
public class ChargeSkill : Skill
{
    [SerializeField] ChargeWarning warningPrefab;
    [SerializeField] float WarningDuration = 0.0f;
    [SerializeField] float ChargeDuration = 0.0f;

    public override void UseSkill(Entity entity, Vector2 dir)
    {
        SkillEntry enemy = entity as SkillEntry;
        base.UseSkill(entity, dir);
        ChargeWarning warning = Instantiate(warningPrefab);
        warning.transform.localPosition = enemy.transform.position;
        warning.Init(entity, enemy.player.transform.position, WarningDuration, ChargeDuration, skillDamage);
    }

}

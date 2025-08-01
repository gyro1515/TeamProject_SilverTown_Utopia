using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Charge Skill Data", menuName = "Scriptable Object/Skill Data/ChargeSkill")]
public class ChargeSkill : Skill
{
    public static bool isChargning = false;
    [SerializeField] ChargeWarning warningPrefab;
    [SerializeField] float warningDuration = 0.0f;
    [SerializeField] float chargeDuration = 0.0f;

    public override void UseSkill(Entity entity, Vector2 dir)
    {
        if (isChargning)
            return;
        SkillEntry enemy = entity as SkillEntry;
        base.UseSkill(entity, dir);
        _coolTime = warningDuration + chargeDuration;
        ChargeWarning warning = Instantiate(warningPrefab);
        warning.transform.localPosition = positionCenter;
        warning.Init(entity, enemy.player.transform.position, warningDuration, chargeDuration, skillDamage);
    }

}

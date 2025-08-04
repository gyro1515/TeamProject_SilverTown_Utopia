using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Charge Skill Data", menuName = "Scriptable Object/Skill Data/ChargeSkill")]
public class ChargeSkill : Skill
{
    //If currently charging
    public static bool isChargning = false;
    //warning copy prefab
    [SerializeField] ChargeWarning warningPrefab;
    //warning duration
    [SerializeField] float warningDuration = 0.0f;
    //charge duration
    [SerializeField] float chargeDuration = 0.0f;

    public override void UseSkill(Entity entity, Vector2 dir)
    {
        //if currently Charging, don't start charge
        if (isChargning)
            return;
        //Set and Active Charge
        Enemy enemy = entity as Enemy;
        base.UseSkill(entity, dir);
        _coolTime = warningDuration + chargeDuration;
        ChargeWarning warning = Instantiate(warningPrefab);
        warning.transform.localPosition = positionCenter;
        warning.Init(entity, enemy.target.transform.position, warningDuration, chargeDuration, skillDamage);
    }

}

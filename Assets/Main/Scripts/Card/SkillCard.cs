using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/SkillCard")]
public class SkillCard : UpgradeCard
{
    [Header("스킬 강화")]
    [SerializeField] int addProjectileCnt = 0;
    public override void ApplySelectedCard()
    {
        Debug.Log("스킬 강화");
        // 예시: Player.Instace.skill ++++
    }
}

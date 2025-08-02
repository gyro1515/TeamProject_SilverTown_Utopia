using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/AbilityCard")]

public class AbilityCard : UpgradeCard
{
    [Header("능력치 강화")]
    [SerializeField]int plusHp = 0;
    [SerializeField]int plusAtk = 0;
    [SerializeField]int plusSpeed = 0;

    public override void ApplySelectedCard()
    {
        Debug.Log("능력치 강화");
        // 예시: Player.Instace.maxHp += plusHp;
        //       Player.Instace.curHp += plusHp;
    }
}

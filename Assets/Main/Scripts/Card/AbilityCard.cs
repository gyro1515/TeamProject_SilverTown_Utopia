using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/AbilityCard")]

public class AbilityCard : UpgradeCard
{
    [Header("능력치 강화")]
    Player player;
    [SerializeField]int plusHp = 0;
    [SerializeField]int plusAtk = 0;
    [SerializeField]int plusSpeed = 0;

    public override void ApplySelectedCard()
    {
        Debug.Log("능력치 강화");
        if (plusHp != 0)
        {
            player.UpgradeHP(plusHp);
        }
        if (plusAtk != 0)
        {
            player.UpgradeAtk(plusAtk);
        }
        if (plusSpeed != 0)
        {
            player.UpgradeSpeed(plusSpeed);
        }
    }

    public void SetCard(Player player)
    {
        this.player = player;
    }
}

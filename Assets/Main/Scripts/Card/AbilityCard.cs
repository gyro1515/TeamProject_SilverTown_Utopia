using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/AbilityCard")]

public class AbilityCard : UpgradeCard
{
    [Header("능력치 강화")]
    Player player;
    [SerializeField]int plusHp = 0;
    [SerializeField] float weaponAtkMultiplier = 1.0f;

    public override void ApplySelectedCard()
    {
        if (plusHp != 0)
        {
            player.UpgradeHP(plusHp);
        }
        if (weaponAtkMultiplier != 1.0f)
        {
            player.SetDamageMultiplier(weaponAtkMultiplier);
        }
    }

    public void SetCard(Player player)
    {
        this.player = player;
    }
}

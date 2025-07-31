using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern Data", menuName = "Scriptable Object/Pattern/FloorPattern")]
public class FloorPattern : Pattern
{
    [SerializeField] int floorCount = 0;
    public override void SetSkills(int i)
    {
        if (i < floorCount)
            base.SetSkills(i);
        else
        {
            skills[i].PositionCenter = skills[i - floorCount].PositionCenter;
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern Data", menuName = "Scriptable Object/Pattern/ScreenPattern")]
public class ScreenPattern : Pattern
{
    Vector2 MapPivot;
    [SerializeField] bool isFullX = true;
    [SerializeField] bool isFullY = true;

    public override void SetSkills(int i)
    {
        base.SetSkills(i);
    }
}

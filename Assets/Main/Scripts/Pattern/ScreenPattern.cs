using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Pattern Data", menuName = "Scriptable Object/Pattern/ScreenPattern")]
public class ScreenPattern : Pattern
{
    // 기존 구조로 스크린 패턴 구현하기 힘들 거 같아서 띠로 만듭니다.
    [Header("스크린 패턴용 스킬")]
    [SerializeField] GameObject screenSkillPrefab = null;
    ScreenSkill screenSkill = null;

    public override void Init()
    {
        isCoolTime = true; // 시작하자마자 스킬 난사 못하도록 쿨타임 돌아가게 하기
        screenSkill = Instantiate(screenSkillPrefab, enemy.gameObject.transform).GetComponent<ScreenSkill>();
        screenSkill.Init(enemy);
    }

    public void UseScreenSkillPattern()
    {
        screenSkill?.UseScreenSkill();
    }

    /*Vector2 mapPivot;
    [SerializeField] bool isFullX = true;
    [SerializeField] bool isFullY = true;

    public override void SetSkills(int i)
    {
        base.SetSkills(i);
    }*/
}

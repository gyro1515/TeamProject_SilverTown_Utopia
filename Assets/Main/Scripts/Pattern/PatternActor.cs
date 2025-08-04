using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Pattern;


public class PatternActor : MonoBehaviour
{
    //Attack Direction
    Vector2 dir;
    //enemy who shoots this pattern
    [SerializeField] Enemy enemy;
    //Pattern which will be used
    //<=============== Need To be Fixed : Pattern to PatternList ===============>
    [SerializeField] public List<Pattern> patternList;
    
    //Copy Pattern
    private void Start()
    {
        enemy = transform.parent.GetComponent<Enemy>();
        for (int i = 0; i < patternList.Count; i++)
        {
            patternList[i] = Instantiate(patternList[i]);
            patternList[i].enemy = enemy;
            patternList[i].Init();
        }
    }
    private void Update()
    {
        if (enemy.BossState != Enemy.EBossState.Active) return; // 활성화될때만 스킬 쿨타임 돌기
        for (int i = 0; i < patternList.Count; i++)
        {
            //Debug.Log($"Pattern{i}: {patternList[i].isCoolTime} -> {patternList[i].patternCoolTimer} / {patternList[i].patternCoolTime}");
            if (!patternList[i].isCoolTime) continue; // 해당 패턴 쿨타임 이면 다음
            patternList[i].patternCoolTimer += Time.deltaTime;
            if(patternList[i].patternCoolTimer >= patternList[i].patternCoolTime)
            {
                patternList[i].patternCoolTimer = 0.0f;
                patternList[i].isCoolTime = false;
            }
        }
    }
    //Active Pattern skill Sequence based on Pattern
    public IEnumerator ActivePattern(int index)
    {
        patternList[index].isCoolTime = true;
        if(patternList[index].isScreenPattern) // 스크린 스킬은 별로도 구현하기
        {
            ((ScreenPattern)patternList[index]).UseScreenSkillPattern();
        }
        else
        {
            for (int i = 0; i < patternList[index].skills.Length; i++)
            {
                dir = enemy.target.transform.position - enemy.transform.position;
                dir = dir.normalized;
                patternList[index].SetSkills(i);
                patternList[index].skills[i].UseSkill(enemy, dir);
                if (i + 1 == patternList[index].skills.Length)
                    break;
                float waitdelay = patternList[index].activeTime[i + 1] - patternList[index].activeTime[i];
                if (patternList[index].ignoreSkillCooldown)
                {
                    if (waitdelay < 0)
                        waitdelay = 0;
                    yield return new WaitForSeconds(waitdelay);
                }
                else
                {
                    waitdelay += patternList[index].skills[i].coolTime;
                    if (waitdelay < 0)
                        waitdelay = 0;
                    yield return new WaitForSeconds(waitdelay);
                }
            }
        }
    }
}

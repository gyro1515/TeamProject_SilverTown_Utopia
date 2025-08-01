using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Pattern;


public class PatternActor : MonoBehaviour
{
    //Attack Direction
    Vector2 dir;
    //enemy who shoots this pattern
    [SerializeField] SkillEntry enemy;
    //Pattern which will be used
    //<=============== Need To be Fixed : Pattern to PatternList ===============>
    [SerializeField] Pattern pattern;

    //Copy Pattern
    private void Start()
    {
        enemy = GetComponent<SkillEntry>();
        pattern = Instantiate(pattern);
        pattern.enemy = enemy;
        pattern.Init();
    }
    //Active Pattern skill Sequence based on Pattern
    public IEnumerator ActivePattern()
    {
        for (int i = 0; i < pattern.skills.Length; i++)
        {
            dir = enemy.player.transform.position - enemy.transform.position;
            dir = dir.normalized;
            pattern.SetSkills(i);
            pattern.skills[i].UseSkill(enemy, dir);
            if (i + 1 == pattern.skills.Length)
                break;
            float waitdelay = pattern.activeTime[i + 1] - pattern.activeTime[i];
            if (pattern.ignoreSkillCooldown)
            {
                if (waitdelay < 0)
                    waitdelay = 0;
                yield return new WaitForSeconds(waitdelay);
            }
            else
            {
                waitdelay += pattern.skills[i].coolTime;
                if (waitdelay < 0)
                    waitdelay = 0;
                yield return new WaitForSeconds(waitdelay);
            }
        }
    }
}

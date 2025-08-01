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
    //Active Pattern skill Sequence based on Pattern
    public IEnumerator ActivePattern(int index)
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

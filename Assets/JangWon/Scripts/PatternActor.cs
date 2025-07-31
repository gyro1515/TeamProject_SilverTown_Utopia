using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Pattern;


public class PatternActor : MonoBehaviour
{
    [SerializeField] SkillEntry enemy;
    [SerializeField] Pattern pattern;

    private void Start()
    {
        enemy = GetComponent<SkillEntry>();
        pattern = Instantiate(pattern);
        pattern.enemy = enemy;
        pattern.Init();
    }
    public IEnumerator ActivePattern()
    {
        for (int i = 0; i < pattern.skills.Length; i++)
        {
            Vector2 dir;
            dir = enemy.player.transform.position - enemy.transform.position;


            pattern.SetSkills(i);
            pattern.skills[i].UseSkill(enemy, dir);
            if (i + 1 == pattern.skills.Length)
                break;
            float waitDelay = pattern.activeTime[i + 1] - pattern.activeTime[i];
            if (pattern.ignoreSkillCooldown)
            {
                if (waitDelay < 0)
                    waitDelay = 0;
                yield return new WaitForSeconds(waitDelay);
            }
            else
            {
                waitDelay += pattern.skills[i].coolTime;
                if (waitDelay < 0)
                    waitDelay = 0;
                yield return new WaitForSeconds(waitDelay);
            }
        }
        if (enemy.isPatternEnd)
        {
            enemy.isPatternEnd = true;
            Debug.Log("Pattern end success");
        }
    }
}

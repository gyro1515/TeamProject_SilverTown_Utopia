using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Pattern;


public class PatternActor : MonoBehaviour
{
    [SerializeField] SkillEntry Shooter;
    [SerializeField] Pattern pattern;

    private void Awake()
    {
        Shooter = GetComponent<SkillEntry>();
        pattern = Instantiate(pattern);
    }

    public IEnumerator ActivePattern()
    {
        Shooter.isPatternEnd = false;
        for (int i = 0; i < pattern.skills.Length; i++)
        {
            Vector2 dir;
            if (pattern.positionStates[i] == Pattern.PositionState.Player 
                || pattern.positionStates[i] == Pattern.PositionState.PlayerRandom 
                || pattern.positionStates[i] == Pattern.PositionState.PlayerFixed)
                dir = Shooter.player.transform.position - Shooter.transform.position;
            else
                dir = Vector2.zero;


            switch (pattern.positionStates[i])
            {
                case PositionState.Origin:
                    pattern.skills[i].PositionCenter = Vector3.zero; break;
                case PositionState.Fixed:
                    pattern.skills[i].PositionCenter = pattern.positions[i]; break;
                case PositionState.Player:
                    pattern.skills[i].PositionCenter = Shooter.player.transform.position; break;
                case PositionState.PlayerFixed:
                    pattern.skills[i].PositionCenter = (Vector2)Shooter.player.transform.position + pattern.positions[i]; break;
                case PositionState.PlayerRandom:
                    Vector2 randompos = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
                    pattern.skills[i].PositionCenter = (Vector2)Shooter.player.transform.position + randompos; break;
            }

            pattern.skills[i].UseSkill(Shooter, dir);
            if (i+1 == pattern.skills.Length)
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
        if (Shooter.isPatternEnd)
            Debug.Log("Pattern end success");
        else
            Debug.Log("Pattern end failed");
        StopCoroutine(ActivePattern());
    }
}

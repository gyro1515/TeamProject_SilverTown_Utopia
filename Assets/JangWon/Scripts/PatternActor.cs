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
        for (int i = 0; i < pattern.skills.Length - 1; i++)
        {
            Vector2 dir;
            if (pattern.positionStates[i] == Pattern.PositionState.Player 
                || pattern.positionStates[i] == Pattern.PositionState.PlayerRandom 
                || pattern.positionStates[i] == Pattern.PositionState.PlayerFixed)
                dir = pattern.Player.transform.position - Shooter.transform.position;
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
            if (i == pattern.skills.Length -1)
                break;
            yield return new WaitForSeconds(pattern.activeTime[i + 1] - pattern.activeTime[i]);
        }
        Shooter.isPatternEnd = true;
        StopCoroutine(ActivePattern());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern Data", menuName = "Scriptable Object/Pattern/BasePattern")]
public class Pattern : ScriptableObject
{
    public enum PositionState
    {
        Origin = 0,
        Fixed,
        Player,
        PlayerRandom,
        PlayerFixed,
        Random
    }
    [SerializeField] public SkillEntry enemy;
    [SerializeField] public Skill[] skills;
    [SerializeField] public float[] activeTime;
    [SerializeField] public PositionState[] positionStates;
    [SerializeField] public Vector2[] positions;
    [SerializeField] public bool ignoreSkillCooldown = true;
    [SerializeField] public Vector2 randomMin = Vector2.zero;
    [SerializeField] public Vector2 randomMax = Vector2.zero;


    private void Start()
    {
        int count = skills.Length;
        if (count != activeTime.Length)
        {
            Debug.Log("Unmatched List size of skill and active time");
            return;
        }
        if (count != positionStates.Length)
        {
            Debug.Log("Unmatched List size of skill and position state");
            return;
        }
        if (count != positions.Length)
        {
            Debug.Log("Unmatched List size of skill and position");
            return;
        }
    }

    public void Init()
    {
        int count = skills.Length;

        for (int i = 0; i < count; i++)
        {
            skills[i] = Instantiate(skills[i]);
        }
    }

    public virtual void SetSkills(int i)
    {
        Vector2 randompos = new Vector2(Random.Range(randomMin.x, randomMax.x), Random.Range(randomMin.y, randomMax.y));
        switch (positionStates[i])
        {
            case PositionState.Origin:
                skills[i].PositionCenter = Vector3.zero; break;
            case PositionState.Fixed:
                skills[i].PositionCenter = positions[i]; break;
            case PositionState.Player:
                skills[i].PositionCenter = enemy.player.transform.position; break;
            case PositionState.PlayerFixed:
                skills[i].PositionCenter = (Vector2)enemy.player.transform.position + positions[i]; break;
            case PositionState.PlayerRandom:
                skills[i].PositionCenter = (Vector2)enemy.player.transform.position + randompos; break;
            case PositionState.Random:
                skills[i].PositionCenter = randompos; break;
        }
    }

}

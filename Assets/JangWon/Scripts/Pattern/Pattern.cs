using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern Data", menuName = "Scriptable Object/Pattern/BasePattern")]
public class Pattern : ScriptableObject
{
    public enum PositionState
    {
        WorldOrigin = 0,
        WorldFixed,
        WorldRandom,
        PlayerOrigin,
        PlayerFixed,
        PlayerRandom,
        EntityOrigin,
        EntityFixed,
        EntityRandom,
        EntityToPlayerScale,
        FixPlayerX,
        FixPlayerY,
        None
    }
    [SerializeField] public SkillEntry enemy;
    [SerializeField] public Skill[] skills;
    [SerializeField] public float[] activeTime;
    [SerializeField] public PositionState[] positionStates;
    [SerializeField] public Vector2[] positions;
    [SerializeField] public bool ignoreSkillCooldown = true;
    [SerializeField] public Vector2 randomMin = Vector2.zero;
    [SerializeField] public Vector2 randomMax = Vector2.zero;
    [SerializeField] public float entityToPlayerScale = 0.5f;
    Vector2 dir;

    float fixedX = 0.0f;
    float fixedY = 0.0f;


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
            case PositionState.WorldOrigin:
                skills[i].PositionCenter = Vector3.zero; break;
            case PositionState.WorldFixed:
                skills[i].PositionCenter = positions[i]; break;
            case PositionState.WorldRandom:
                skills[i].PositionCenter = randompos; break;
            case PositionState.PlayerOrigin:
                skills[i].PositionCenter = enemy.player.transform.position; break;
            case PositionState.PlayerFixed:
                skills[i].PositionCenter = (Vector2)enemy.player.transform.position + positions[i]; break;
            case PositionState.PlayerRandom:
                skills[i].PositionCenter = (Vector2)enemy.player.transform.position + randompos; break;
            case PositionState.EntityOrigin:
                skills[i].PositionCenter = (Vector2)enemy.transform.position; break;
            case PositionState.EntityFixed:
                skills[i].PositionCenter = (Vector2)enemy.transform.position + positions[i]; break;
            case PositionState.EntityRandom:
                skills[i].PositionCenter = (Vector2)enemy.player.transform.position + randompos; break;
            case PositionState.EntityToPlayerScale:
                Vector2 midPoint = Vector2.Lerp(enemy.transform.position, enemy.player.transform.position, entityToPlayerScale);
                skills[i].PositionCenter = midPoint; break;
            case PositionState.FixPlayerX:
                if (i == 0)
                    fixedX = enemy.player.transform.position.x;
                skills[i].PositionCenter = new Vector2(fixedX, 0) + positions[i]; break;
            case PositionState.FixPlayerY:
                if (i == 0)
                    fixedY = enemy.player.transform.position.y;
                skills[i].PositionCenter = new Vector2(0, fixedY) + positions[i]; break;
        }
    }

}

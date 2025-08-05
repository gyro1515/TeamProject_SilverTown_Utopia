using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern Data", menuName = "Scriptable Object/Pattern/BasePattern")]
public class Pattern : ScriptableObject
{
    public enum PositionState
    {
        WorldOrigin = 0, // World Position 0,0,0
        WorldFixed, // World Position from positions
        WorldRandom, // World Postion from random Range
        PlayerOrigin, // Player Position 0,0,0
        PlayerFixed, // Player Position + fixed positions offset
        PlayerRandom, // Player Position + random Range
        EntityOrigin, // shooter Position 0,0,0
        EntityFixed, // shooter Position + fixed positions offset
        EntityRandom, // shooter Position + random Range
        EntityToPlayerScale, // Spawn Between Entity and Player based on entityToPlayerScale
        FixPlayerX, // Fix Player position X and apply this X position for later use + positions.Y
        FixPlayerY, // Fix Player position Y and apply this Y position for later use + positions.X
        TakePreviousPosOffset, //position is same as positions[index - previousPosIndex]
        TakePreviousPosFixed, //position is same as positions[previousPosIndex]
        None //Don't do any operation, Used in EndSkill Or Speciallized override
    }
    //Shooter enemy
    [SerializeField] public Enemy enemy;
    //Array of Squential skills
    [SerializeField] public Skill[] skills;
    //Array of each skills active time
    [SerializeField] public float[] activeTime;
    //Array of each skill's positionStates
    [SerializeField] public PositionState[] positionStates;
    //Array of each skill's positions
    [SerializeField] public Vector2[] positions;
    //if Ture, ignore skill cooldown and next skill waits until activeTime
        //if True, manually set cooldown
    //if False, wait skill cooldown + activeTime
        //if False, activeTime can be negative
    [SerializeField] public bool ignoreSkillCooldown = true;
    //Random range min values
    [SerializeField] public Vector2 randomMin = Vector2.zero;
    //Random range max values
    [SerializeField] public Vector2 randomMax = Vector2.zero;
    //offset of Player to Entity -> Used in EntityToPlayerScale
        //0.0 is Entity Position <-------> 1.0 is Player Position
    [SerializeField] public float entityToPlayerScale = 0.5f;
    //PreviousPosIndex used in TakePreviousPosOffset and TakePreviousPosFixed
        //TakePreviousPosOffset - Set this value to offset
        //TakePreviousPosFixed - Set this value to index you will directly access
    [SerializeField] public int previousPosIndex = 0;
    //Attack Direction
    Vector2 dir;
    //Saved value for FixPlayerX
    float fixedX = 0.0f;
    //Saved value for FixPlayerY
    float fixedY = 0.0f;
    //Saved value for FixPlayer
    Vector3 fixedV3 = Vector3.zero;
    // 패턴 당 쿨타임 설정하기
    // 쿨타임 용
    [SerializeField] public float patternCoolTime = 5.0f;
    [HideInInspector] public float patternCoolTimer = 0.0f;
    [HideInInspector] public bool isCoolTime = true;
    // 스크린 패턴 용 함수
    [SerializeField] public bool isScreenPattern = false;


    //If unequal Array Size, Error
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
    
    //Copy all Skills
    public virtual void Init()
    {
        int count = skills.Length;

        for (int i = 0; i < count; i++)
        {
            skills[i] = Instantiate(skills[i]);
        }
        isCoolTime = true; // 시작하자마자 스킬 난사 못하도록 쿨타임 돌아가게 하기
    }

    //Set each skill's Positions
    public virtual void SetSkills(int i)
    {
        Vector2 randompos = new Vector2(Random.Range(randomMin.x, randomMax.x), Random.Range(randomMin.y, randomMax.y));

        switch (positionStates[i])
        {
            case PositionState.WorldOrigin:
                skills[i].positionCenter = Vector3.zero; break;
            case PositionState.WorldFixed:
                skills[i].positionCenter = positions[i]; break;
            case PositionState.WorldRandom:
                skills[i].positionCenter = randompos; break;
            case PositionState.PlayerOrigin:
                skills[i].positionCenter = enemy.target.transform.position; break;
            case PositionState.PlayerFixed:
                skills[i].positionCenter = (Vector2)enemy.target.transform.position + positions[i]; break;
            case PositionState.PlayerRandom:
                skills[i].positionCenter = (Vector2)enemy.target.transform.position + randompos; break;
            case PositionState.EntityOrigin:
                skills[i].positionCenter = (Vector2)enemy.transform.position; break;
            case PositionState.EntityFixed:
                skills[i].positionCenter = (Vector2)enemy.transform.position + positions[i]; break;
            case PositionState.EntityRandom:
                skills[i].positionCenter = (Vector2)enemy.target.transform.position + randompos; break;
            case PositionState.EntityToPlayerScale:
                Vector2 midPoint = Vector2.Lerp(enemy.transform.position, enemy.target.transform.position, entityToPlayerScale);
                skills[i].positionCenter = midPoint; break;
            case PositionState.FixPlayerX:
                /*if (i == 0)
                    fixedX = enemy.target.transform.position.x;
                skills[i].positionCenter = new Vector2(fixedX, 0) + positions[i]; break;*/
                // X가 고정이라고 플레이어 X값만 가져오면 안됨 -> 플레이어 좌표에서 + positions[i]해서 Y값만 다르게 해야 함
                // 위 방식대로 하면 플레이어의 X와 Y = 0에서 + positions[i] 계산됨...
                //if (i == 0) fixedV3 = enemy.target.transform.position;
                fixedV3 = enemy.target.transform.position;
                skills[i].positionCenter = fixedV3 + (Vector3)positions[i]; break;
            case PositionState.FixPlayerY:
                //if (i == 0)
                //    fixedY = enemy.target.transform.position.y;
                //skills[i].positionCenter = new Vector2(0, fixedY) + positions[i]; break;

                // 위와 돌일
                if (i == 0) fixedV3 = enemy.target.transform.position;
                skills[i].positionCenter = fixedV3 + (Vector3)positions[i]; break;

            case PositionState.TakePreviousPosOffset:
                if (i < previousPosIndex)
                {
                    Debug.Log("Index out of range : Previous Index Offset");
                    return;
                }
                skills[i].positionCenter = skills[i - previousPosIndex].positionCenter; break;
            case PositionState.TakePreviousPosFixed:
                if (i <= previousPosIndex)
                {
                    Debug.Log("Index out of range : Previous Index Offset");
                    return;
                }
                skills[i].positionCenter = skills[previousPosIndex].positionCenter; break;
        }
    }

}

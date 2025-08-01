using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bahamut : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        behaviorTreeRoot = SetBehaviorTree();
    }
    
    protected override INode SetBehaviorTree()
    {
        ActionNode isActive = new ActionNode(IsActive); // 액션 노드는 함수 바인딩
        ActionNode moveToCenter = new ActionNode(MoveToCenter);
        ActionNode canAttack = new ActionNode(CanAttack); // 액션 노드는 함수 바인딩
        // 아래는 노션의 패턴 순서랑 다릅니다. 비헤이비어 트리 특성상 첫번째로 체크하는 패턴이 가장 강해야 할겁니다.
        // -> 패턴 사용 가능 여부 체크를 1 -> 2 -> 3 -> 4 순으로 하기 때문에 1, 2, 3, 4 쿨타임이 다 돌아와도 1부터 사용합니다.
        // 패턴 1: 아마 가장 강한 스킬을 넣어야하지 않을까 싶습니다.
        ActionNode attack1 = new ActionNode(Attack1); // 액션 노드는 함수 바인딩
        WaitNode attack1Time = new WaitNode(3.0f); // 공격 지속 시간에 해당, 공격하고 일정 시간 비헤이비어 트리 상태 유지
        SequenceNode attack1Sequence = new SequenceNode(new List<INode>() { attack1, attack1Time }); // 공격이 끝나야 성공처리
        // 패턴 2
        ActionNode attack2 = new ActionNode(Attack2); 
        WaitNode attack2Time = new WaitNode(2.0f); 
        SequenceNode attack2Sequence = new SequenceNode(new List<INode>() { attack2, attack2Time });
        // 패턴 3
        ActionNode attack3 = new ActionNode(Attack3); 
        WaitNode attack3Time = new WaitNode(2.0f); 
        SequenceNode attack3Sequence = new SequenceNode(new List<INode>() { attack3, attack3Time }); 
        // 패턴 4
        ActionNode attack4 = new ActionNode(Attack4); 
        WaitNode attack4Time = new WaitNode(2.0f); 
        SequenceNode attack4Sequence = new SequenceNode(new List<INode>() { attack4, attack4Time });

        // 패턴 셀렉터, 패턴 중 사용 가능한 패턴부터 사용.
        SelectorNode patternSelector = new SelectorNode(new List<INode>() { attack1Sequence, attack2Sequence, attack3Sequence, attack4Sequence });
        ActionNode chase = new ActionNode(Chase);
        SequenceNode attackSequence = new SequenceNode(new List<INode>() { canAttack, patternSelector });
        SequenceNode activeSequence = new SequenceNode(new List<INode>() { isActive, moveToCenter });
        SelectorNode selector = new SelectorNode(new List<INode>() { activeSequence, attackSequence, chase }); // 루트에 해당

        return selector;
    }

    protected override void SetDirection()
    {
        throw new System.NotImplementedException();
    }

    INode.ENodeState Chase()
    {
        //Debug.Log("Chase");
        if (Vector3.Distance(target.transform.position, transform.position) <= attackRange) // 공격 범위라면 가만히 있기
        {
            moveDirection = Vector2.zero;
            return INode.ENodeState.Success;
        }
        else if (Vector3.Distance(target.transform.position, transform.position) <= chaseRange) // 범위 안이라면 쫓아가기
        {
            //Debug.Log("ChaseSuccess");
            moveDirection = (target.transform.position - transform.position).normalized;
            MoveSpeed = baseSpeed; // 가까워지면 기본 속도로

            // 회전 하면서 쫓아가기
            /*Vector3 dir = target.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180f;*/
            //transform.rotation = Quaternion.Euler(0, 0, angle);

            return INode.ENodeState.Success;
        }
        else if (Vector3.Distance(target.transform.position, transform.position) <= chaseRange * 2.0f) // 범위 * 2 안이라면 타겟 스피드의 2배로 쫓아가기
        {
            //Debug.Log("ChaseSuccess");
            moveDirection = (target.transform.position - transform.position).normalized;
            MoveSpeed = target.GetComponent<Player>().MoveSpeed * 2; // 타겟 스피드의 2배로

            // 회전 하면서 쫓아가기
            /*Vector3 dir = target.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180f;*/
            //transform.rotation = Quaternion.Euler(0, 0, angle);

            return INode.ENodeState.Success;
        }
        else
        {
            //Debug.Log("ChaseFailure");
            moveDirection = Vector2.zero;
            MoveSpeed = baseSpeed;
            return INode.ENodeState.Failure; // 이 다음에 행동 노드가 있다면 다른 걸 해야할 수도 있다. 지금은 상관 없다.

        }

    }
    INode.ENodeState CanAttack()
    {
        //Debug.Log("CanAttack");

        // 공격 범위 내 혹은 공격 중이라면 성공 판정
        //if (Vector3.Distance(target.transform.position, transform.position) <= attackRange || isAttacking)
        if (Vector3.Distance(target.transform.position, transform.position) <= attackRange)
        {
            //Debug.Log("CanAttackSuccess");
            moveDirection = Vector2.zero;
            return INode.ENodeState.Success;
        }

        return INode.ENodeState.Failure;
    }
    INode.ENodeState Attack1()
    {
        // 여기서 스킬(패턴) 사용!
        // 스킬 쿨타임 여부
        bool canUseSkill = Random.Range(0, 2) == 0 ? true : false;
        if (canUseSkill) // 쿨타임이 다 돌았다면
        {
            Debug.Log("스킬 패턴1 사용!!");
            return INode.ENodeState.Success;
        }
        else
        {
            Debug.Log("스킬 패턴1은 쿨타임입니다.");
            return INode.ENodeState.Failure;
        }

        /*if (!isAttacking) // 공격 중이 아니라면 
        {
            //Debug.Log("공격 시작");
            isAttacking = true;
            // 이 시점에 애니메이션이나 이펙트 실행도 가능
            // animator.SetTrigger("Attack");
            // 회전 하면서 쫓아가기
            *//*Vector3 dir = target.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);*//*
            return INode.ENodeState.Running; // 진행 중 표시, 노드가 다른 곳으로 안가도록
        }
        isAttacking = false;*/
    }
    INode.ENodeState Attack2()
    {
        // 여기서 스킬(패턴) 사용!
        // 스킬 쿨타임 여부
        bool canUseSkill = Random.Range(0, 2) == 0 ? true : false;
        if (canUseSkill) // 쿨타임이 다 돌았다면
        {
            Debug.Log("스킬 패턴2 사용!!");
            return INode.ENodeState.Success;
        }
        else
        {
            Debug.Log("스킬 패턴2은 쿨타임입니다.");
            return INode.ENodeState.Failure;
        }
    }
    INode.ENodeState Attack3()
    {
        // 스킬 쿨타임 여부
        bool canUseSkill = Random.Range(0, 2) == 0 ? true : false;
        if (canUseSkill) // 쿨타임이 다 돌았다면
        {
            Debug.Log("스킬 패턴3 사용!!");
            return INode.ENodeState.Success;
        }
        else
        {
            Debug.Log("스킬 패턴3은 쿨타임입니다.");
            return INode.ENodeState.Failure;
        }
    }
    INode.ENodeState Attack4()
    {
        // 스킬 쿨타임 여부
        bool canUseSkill = Random.Range(0, 2) == 0 ? true : false;
        if (canUseSkill) // 쿨타임이 다 돌았다면
        {
            Debug.Log("스킬 패턴4 사용!!");
            return INode.ENodeState.Success;
        }
        else
        {
            Debug.Log("스킬 패턴4은 쿨타임입니다.");
            return INode.ENodeState.Failure;
        }
    }
    INode.ENodeState Attack5() // 만약 랜덤이라면 아래 형식을 따르면 될겁니다.
    {
        // 랜덤으로 스킬 고르기
        bool canUseSkill = Random.Range(0, 2) == 0 ? true : false;
        if(canUseSkill) // 사용 가능한 스킬이라면
        {
            Debug.Log("사용 가능한 스킬 사용!!");
            return INode.ENodeState.Success;
        }
        else
        {
            Debug.Log("해당 스킬은 쿨타임입니다.");
            return INode.ENodeState.Failure;
        }
        

    }
    INode.ENodeState IsActive()
    {
        //Debug.Log("IsActive");
        if (BossState == EBossState.Deactivate)
        {
            return INode.ENodeState.Success;
        }
        else // if (BossState == EBossState.Active)
        {
            return INode.ENodeState.Failure;
        }

    }
    INode.ENodeState MoveToCenter()
    {
        //Debug.Log("MoveToCenter");
        MoveSpeed = baseSpeed;
        if ((Vector2)transform.position != myRoom.Room.center)
        {
            // 아직 가운데에 도달 안했다면 러닝
            Vector2 tmpDir = myRoom.Room.center - (Vector2)transform.position;
            // 일정 거리 안이라면 그냥 가운데로 가게끔
            if (tmpDir.magnitude > 0.1f)
            {
                moveDirection = tmpDir.normalized;
                //transform.position += (Vector3)(tmpDir.normalized * MoveSpeed * Time.deltaTime);
            }
            else
            {
                moveDirection = Vector2.zero;
                transform.position = myRoom.Room.center;
            }
            //return INode.ENodeState.Running; // 러닝으로 하면 돌아가는 도중 맵 입장시 보스 활성화 안됨
            return INode.ENodeState.Success;
        }
        MoveAnimation(Vector2.down);
        //Debug.Log("MoveToCenterSuccess");
        BossState = EBossState.PowerOff;
        return INode.ENodeState.Success;
    }
}

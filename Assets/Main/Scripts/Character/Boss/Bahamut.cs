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
        ActionNode attack = new ActionNode(Attack); // 액션 노드는 함수 바인딩
        CooldownNode attackCoolTime = new CooldownNode(attack, 3.0f); // 액션 노드와 해당 액션 노드의 쿨타임 설정
        WaitNode attackTime = new WaitNode(2.0f); // 공격 지속 시간에 해당, 공격하고 일정 시간 비헤이비어 트리 상태 유지
        ActionNode chase = new ActionNode(Chase);
        SequenceNode attackSequence = new SequenceNode(new List<INode>() { canAttack, attackCoolTime, attackTime });
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
    INode.ENodeState Attack()
    {
        // 여기서 스킬(패턴) 사용!
        Debug.Log("스킬 패턴 사용!!");

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
        return INode.ENodeState.Success;
    }
    INode.ENodeState IsActive()
    {
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
        moveDirection = Vector2.zero;
        MoveSpeed = baseSpeed;
        if ((Vector2)transform.position != myRoom.Room.center)
        {
            // 아직 가운데에 도달 안했다면 러닝
            Vector2 tmpDir = myRoom.Room.center - (Vector2)transform.position;
            // 일정 거리 안이라면 그냥 가운데로 가게끔
            if (tmpDir.magnitude > 0.1f)
            {
                transform.position += (Vector3)(tmpDir.normalized * MoveSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = myRoom.Room.center;
            }
            //return INode.ENodeState.Running; // 러닝으로 하면 돌아가는 도중 맵 입장시 보스 활성화 안됨
            return INode.ENodeState.Success;
        }
        //Debug.Log("MoveToCenterSuccess");
        BossState = EBossState.PowerOff;
        return INode.ENodeState.Success;
    }
}

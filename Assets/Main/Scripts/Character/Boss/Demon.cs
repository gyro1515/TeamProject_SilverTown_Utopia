using System.Collections.Generic;
using UnityEngine;

public class Demon : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        behaviorTreeRoot = SetBehaviorTree();

    }
    protected override INode SetBehaviorTree()
    {
        ActionNode isActive = new ActionNode(IsActive);  // 활성화 여부 체크
        ActionNode moveToCenter = new ActionNode(MoveToCenter); // Deactivate라면 보스 위치 초기화
        SequenceNode activeSequence = new SequenceNode(new List<INode>() { isActive, moveToCenter });

        // patternSequence 내용
        ActionNode canAttack = new ActionNode(CanAttack); // 공격 범위 && 패턴 사용 가능 체크
        // 패턴 1:
        ActionNode attack1 = new ActionNode(Attack1);
        WaitNode attack1Time = new WaitNode(2.0f); // 공격 지속 시간에 해당, 공격하고 일정 시간 비헤이비어 트리 상태 유지
        SequenceNode attack1Sequence = new SequenceNode(new List<INode>() { attack1, attack1Time }); // 공격이 끝나야 성공처리
        // 패턴 2
        ActionNode attack2 = new ActionNode(Attack2);
        WaitNode attack2Time = new WaitNode(1.5f);
        SequenceNode attack2Sequence = new SequenceNode(new List<INode>() { attack2, attack2Time });
        // 패턴 3
        ActionNode attack3 = new ActionNode(Attack3);
        WaitNode attack3Time = new WaitNode(1.0f);
        SequenceNode attack3Sequence = new SequenceNode(new List<INode>() { attack3, attack3Time });
        /*// 패턴 4
        ActionNode attack4 = new ActionNode(Attack4);
        WaitNode attack4Time = new WaitNode(1.5f);
        SequenceNode attack4Sequence = new SequenceNode(new List<INode>() { attack4, attack4Time });*/
        // 패턴 셀렉터, 패턴 중 사용 가능한 패턴부터 사용.
        SelectorNode patternSelector = new SelectorNode(new List<INode>() { attack1Sequence, attack2Sequence, attack3Sequence }); // 쿨타임 긴 순서대로 설정하기
        // 패턴 시전 가능 체크하고, 사용 가능한 패턴들 체크(패턴 쿨타임 체크)
        SequenceNode patternSequence = new SequenceNode(new List<INode>() { canAttack, patternSelector });

        // ChaseSel 내용
        ActionNode chase = new ActionNode(Chase); // 일정 범위 안이라면 느릿하게 플레이어 쫓기
        WaitNode chaseTime = new WaitNode(0.5f); // 일정 시간마다 보스의 움직임 방향 바꾸기
        SequenceNode chaseSeq = new SequenceNode(new List<INode>() { chase, chaseTime }); // 방향 정하고, 
        ActionNode dashToPlayerPos = new ActionNode(DaseToPlayerPos);
        SelectorNode chaeSel = new SelectorNode(new List<INode>() { chaseSeq, dashToPlayerPos });

        SelectorNode selector = new SelectorNode(new List<INode>() { activeSequence, patternSequence, chaeSel }); // 루트에 해당

        return selector;
    }

    protected override void SetDirection()
    {
        throw new System.NotImplementedException();
    }

    INode.ENodeState CanAttack()
    {

        // 공격 범위 내이면서 패턴 사용 가능 상태라면 성공판정
        if (Vector3.Distance(target.transform.position, transform.position) <= attackRange && this.isPatternEnd) // isPatternEnd도 같이 확인
        {
            //Debug.Log("CanAttackSuccess");
            moveDirection = Vector2.zero;
            Vector2 lookDir = (target.transform.position - transform.position);
            // 애니메이션 회전, 부모에서 MoveAnimation(moveDirection)을 실행하더라도 moveDirection = Vector2.zero이기때문에 lookDir회전값으로 적용됨
            MoveAnimation(lookDir);
            return INode.ENodeState.Success;
        }

        return INode.ENodeState.Failure;
    }
    INode.ENodeState Attack1()
    {
        if (this.actor.patternList.Count <= 0 || this.actor.patternList[0] == null)
        {
            Debug.Log("스킬 패턴1이 없습니다.");
            return INode.ENodeState.Failure;
        }
        // 스킬 쿨타임 여부
        if (!actor.patternList[0].isCoolTime) // 쿨타임이 다 돌았다면
        {
            this.isPatternEnd = false;
            actor.StartCoroutine(actor.ActivePattern(0));
            return INode.ENodeState.Success;
        }
        else
        {
            return INode.ENodeState.Failure;
        }


    }
    INode.ENodeState Attack2()
    {
        // 여기서 스킬(패턴) 사용!
        if (this.actor.patternList.Count <= 1 || this.actor.patternList[1] == null)
        {
            Debug.Log("스킬 패턴2이 없습니다.");
            return INode.ENodeState.Failure;
        }
        if (!actor.patternList[1].isCoolTime) // 쿨타임이 다 돌았다면
        {
            this.isPatternEnd = false;
            actor.StartCoroutine(actor.ActivePattern(1));
            return INode.ENodeState.Success;
        }
        else
        {
            return INode.ENodeState.Failure;
        }
    }
    INode.ENodeState Attack3()
    {
        // 여기서 스킬(패턴) 사용!
        if (this.actor.patternList.Count <= 2 || this.actor.patternList[2] == null)
        {
            Debug.Log("스킬 패턴3이 없습니다.");
            return INode.ENodeState.Failure;
        }
        // 스킬 쿨타임 여부
        if (!actor.patternList[2].isCoolTime) // 쿨타임이 다 돌았다면
        {
            this.isPatternEnd = false;
            actor.StartCoroutine(actor.ActivePattern(2));
            return INode.ENodeState.Success;
        }
        else
        {
            return INode.ENodeState.Failure;
        }
    }
    INode.ENodeState Attack4()
    {
        // 여기서 스킬(패턴) 사용!
        if (this.actor.patternList.Count <= 3 || this.actor.patternList[3] == null)
        {
            Debug.Log("스킬 패턴4이 없습니다.");
            return INode.ENodeState.Failure;
        }
        // 스킬 쿨타임 여부
        if (!actor.patternList[3].isCoolTime) // 쿨타임이 다 돌았다면
        {
            this.isPatternEnd = false;
            actor.StartCoroutine(actor.ActivePattern(3));
            return INode.ENodeState.Success;
        }
        else
        {
            return INode.ENodeState.Failure;
        }
    }


}

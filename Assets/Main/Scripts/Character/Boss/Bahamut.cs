using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EventSystems;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bahamut : Enemy
{
    Vector2 prePlayerPos = Vector2.zero;
    [Header("Bahamut 세팅")]
    [SerializeField] float dashMultiple = 3.0f;
    [SerializeField] float bodyDamage = 1.0f;
    [SerializeField] float bodyDamageTime = 0.1f;
    bool bPlayerInBody = false;
    float bodyDamageTimer = 0.3f; 

    protected CapsuleCollider2D bodyCol; // 기존 콜라이더
    protected CapsuleCollider2D trigerCol; // 몸박용 트리거 콜라이더 설정

    private LayerMask wallLayer; // 벽 감지용
    private float rayDist = -1f; // 벽 감지 레이캐스트용 거리
    // 대시용 bool 변수
    bool bIsDash = false;
    protected override void Awake()
    {
        base.Awake();
        behaviorTreeRoot = SetBehaviorTree();
        bodyCol = GetComponent<CapsuleCollider2D>();
        // 몸 박용, 같은 크기의 캡슐 콜라이더 생성, 플레이어만 충돌판정 나도록
        trigerCol = gameObject.AddComponent<CapsuleCollider2D>();
        trigerCol.isTrigger = true;
        trigerCol.size = bodyCol.size;
        int include = LayerMask.GetMask("Player");
        trigerCol.includeLayers = include; // 포함
        int exclude = 0;
        exclude |= 1 << 0; 
        exclude |= 1 << 1;
        exclude |= 1 << 2;
        exclude |= 1 << 4;
        exclude |= 1 << 5;
        exclude |= 1 << 6;
        exclude |= 1 << 8;
        trigerCol.excludeLayers = exclude; // 제외

        bodyDamageTimer = bodyDamageTime; // 몸박당하자 마자 데미지 주기

        // 레이캐스트 벽 감지용
        wallLayer = LayerMask.GetMask("Wall");
        // 콜라이더 크기에 따른 레이캐스트 거리 구하기
        float halfWidth = bodyCol.size.x / 2f;
        float halfHeight = bodyCol.size.y / 2f;
        rayDist = Mathf.Sqrt(halfWidth * halfWidth + halfHeight * halfHeight);
        rayDist = Mathf.Ceil(rayDist); // 소수점 올림하여 거리 넉넉하게 잡기
    }
    protected override void Update()
    {
        base .Update();
        if (!bPlayerInBody) return;
        AddBodyDamageToTarget();
        
    }
    // 우선 바하뮤트만 충돌 설정
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!bIsDash) return; // 대시 중일때만 판단

        // 벽에 부딪혔을 때 법선 구하기
        //Vector2 wallNormal = collision.contacts[0].normal;
        // 위 방식으로 구하면 위아래로 겹칠때 에러 발생
        
        // 레이캐스트로 벽 탐지
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, rayDist, wallLayer); // 캡슐 크기에 따라 레이캐스트 거리 달라져야함
        Vector2 wallNormal = hit.normal;

        // 만약 법선과 moveDirection의 방향이 180도에 가깝다면 대시 중지
        float angle = Vector2.Angle(wallNormal, moveDirection);
        if (angle > 170f)
        {
            Debug.Log($" 멈춤 각도: {angle}, 법선 백터: {wallNormal}, 레이 거리: {rayDist}");

            prePlayerPos = transform.position;
            return;
        }
        else Debug.Log($" 진행 각도: {angle}, 법선 백터: {wallNormal}, 레이 거리: {rayDist}");

        // 기존 대시 벡터에서 수직 성분 제거 → 벽을 따라 미끄러지는 방향 계산
        Vector2 slideDirection = moveDirection - Vector2.Dot(moveDirection, wallNormal) * wallNormal;
        // 바하뮤트의 이동방향 갱신
        moveDirection = slideDirection.normalized;

        // 남은 거리 구하기
        float dist = Vector2.Distance(transform.position, prePlayerPos);
        // prePlayerPos 갱신
        prePlayerPos = (Vector2)transform.position + moveDirection * dist;
    }
    
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Trigger 몸박시작: {collision.gameObject.name}");
        bPlayerInBody = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log($"Trigger 몸박종료: {collision.gameObject.name}");
        bPlayerInBody = false;
    }

    protected override INode SetBehaviorTree()
    {
        // 액션 노드는 함수 바인딩

        // activeSequence 내용
        // 활성화되었다면 patternSequence체크, 비활성이고 보스가 맵 중앙이 아니라면 보스가 중앙으로 이동 후 PowerOff
        ActionNode isActive = new ActionNode(IsActive);  // 활성화 여부 체크
        ActionNode moveToCenter = new ActionNode(MoveToCenter); // Deactivate라면 보스 위치 초기화
        SequenceNode activeSequence = new SequenceNode(new List<INode>() { isActive, moveToCenter });

        // patternSequence 내용
        ActionNode canAttack = new ActionNode(CanAttack); // 공격 범위 && 패턴 사용 가능 체크
        // 패턴 1:
        ActionNode attack1 = new ActionNode(Attack1);
        WaitNode attack1Time = new WaitNode(3.5f); // 공격 지속 시간에 해당, 공격하고 일정 시간 비헤이비어 트리 상태 유지
        SequenceNode attack1Sequence = new SequenceNode(new List<INode>() { attack1, attack1Time }); // 공격이 끝나야 성공처리
        // 패턴 2
        ActionNode attack2 = new ActionNode(Attack2); 
        WaitNode attack2Time = new WaitNode(3.0f); 
        SequenceNode attack2Sequence = new SequenceNode(new List<INode>() { attack2, attack2Time });
        // 패턴 3
        ActionNode attack3 = new ActionNode(Attack3); 
        WaitNode attack3Time = new WaitNode(2.5f); 
        SequenceNode attack3Sequence = new SequenceNode(new List<INode>() { attack3, attack3Time }); 
        // 패턴 4
        ActionNode attack4 = new ActionNode(Attack4); 
        WaitNode attack4Time = new WaitNode(1.5f); 
        SequenceNode attack4Sequence = new SequenceNode(new List<INode>() { attack4, attack4Time });
        // 패턴 셀렉터, 패턴 중 사용 가능한 패턴부터 사용.
        SelectorNode patternSelector = new SelectorNode(new List<INode>() { attack1Sequence, attack2Sequence, attack3Sequence, attack4Sequence });
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

    INode.ENodeState Chase()
    {
        //Debug.Log("Chase");
       if (Vector3.Distance(target.transform.position, transform.position) <= chaseRange) // 범위 안이라면 쫓아가기
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
        else // 아니라면 대시
        {
            //Debug.Log("ChaseFailureAndDash");
            moveDirection = (target.transform.position - transform.position).normalized;
            // 변수 따로 빼기?**************
            MoveSpeed = target.GetComponent<Player>().MoveSpeed * dashMultiple; // 타겟 스피드의 n배로
            //bodyCol.isTrigger = true;
            //bodyCol.enabled = false;
            bIsDash = true;
            prePlayerPos = target.transform.position;
            return INode.ENodeState.Failure; // 이 다음에 행동 노드가 있다면 다른 걸 해야할 수도 있다. 지금은 상관 없다.
        }

    }
    INode.ENodeState DaseToPlayerPos()
    {
        /*// 대쉬 끝지점에 도착했다면 리턴 성공
        if ((Vector2)transform.position == prePlayerPos)
        {
            MoveSpeed = baseSpeed;
            moveDirection = Vector2.zero;
            return INode.ENodeState.Success;
        }*/

        if (Vector3.Distance(prePlayerPos, transform.position) < 0.5f) // 거리가 일정 거리 미만이라면 위치 갱신 후 리턴 성공
        {
            transform.position = prePlayerPos;
            MoveSpeed = baseSpeed;
            moveDirection = Vector2.zero;
            //bodyCol.isTrigger = false;
            //bodyCol.enabled = true;
            bIsDash = false;

            return INode.ENodeState.Success;
        }
        return INode.ENodeState.Running;
    }
    INode.ENodeState CanAttack()
    {
        //Debug.Log("CanAttack");
        // 공격 범위 내 혹은 공격 중이라면 성공 판정
        //if (Vector3.Distance(target.transform.position, transform.position) <= attackRange || isAttacking)


        // 공격 범위 내이면서 패턴 사용 가능 상태라면 성공판정
        if (Vector3.Distance(target.transform.position, transform.position) <= attackRange) // && SkillEntryd의 isPatternEnd도 같이 확인
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
        // 여기서 스킬(패턴) 사용!
        // 스킬 쿨타임 여부
        bool canUseSkill = UnityEngine.Random.Range(0, 2) == 0 ? true : false;
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
        bool canUseSkill = UnityEngine.Random.Range(0, 2) == 0 ? true : false;
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
        bool canUseSkill = UnityEngine.Random.Range(0, 2) == 0 ? true : false;
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
        bool canUseSkill = UnityEngine.Random.Range(0, 2) == 0 ? true : false;
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
    /*INode.ENodeState Attack5() // 만약 랜덤이라면 아래 형식을 따르면 될겁니다.
    {
        // 패턴을 사용할 수 있는가

        // 사용할 수 있다면
        // 쿨타임 돌아온 것들만 선별해서 랜덤으로 스킬 고르기
        // 만약 쿨타임 돌아온 스킬이 없다면 리턴 실패
        // 있다면 랜덤 선택 후, 스킬 사용, 리턴 성공

        // 사용하고 리턴 성공
        // 사용할 수 없다면
        // 리턴 실패


        bool canUseSkill = Random.Range(0, 4) == 0 ? true : false;
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
    }*/
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
   
    void AddBodyDamageToTarget()
    {
        bodyDamageTimer += Time.deltaTime;
        if (bodyDamageTimer >= bodyDamageTime)
        {
            bodyDamageTimer -= bodyDamageTime;
            //Debug.Log($"플레이어에게 몸박 데미지{bodyDamage}");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Entity
{
    [Header("보스 세팅")]
    [SerializeField] public Player target;
    [SerializeField] protected float attackRange = 3.0f;
    [SerializeField] protected float chaseRange = 10.0f;
    [SerializeField] protected int bodyDamage = 1; // 몸박 데미지
    [SerializeField] protected float bodyDamageTime = 0.1f; // 몸박 데미지 간격
    [SerializeField] protected float dashMultiple = 3.0f; // 대시 스피드 배율(플레이어 * dashMultiple = 대시 스피드)
    [SerializeField] protected PatternActor actor;
    [SerializeField] protected AudioClip bossBGM;
    public AudioClip BossBGM { get { return bossBGM; } }
    protected float baseSpeed; // 몬스터는 이동속도가 변하기 때문에, 베이스 이동속도 따로 추가
    public enum EBossState // 보스 방 진입시 활성화, 탈출 시 비활성화(맵 중앙으로 돌아가기), 중앙으로 돌아가면 작동 안하도록(업데이트x)
    {
        Active, Deactivate, PowerOff
    }
    protected EBossState bossState = EBossState.Deactivate;
    public EBossState BossState { set { bossState = value; } get { return bossState; } }
    protected BossRoom myRoom = null; // 현재 보스가 속해있는 방
    public BossRoom MyRoom { get { return myRoom; } private set { } }
    protected INode behaviorTreeRoot; // 비헤이비어 트리 루트
    //protected bool isAttacking; // 공격 중인지 여부, 비헤이비어 트리로 구현하기
        
    // 몸박용
    protected bool bPlayerInBody = false;
    protected float bodyDamageTimer = 0.3f; // 몸박 데미지 간격 타이머
    protected Vector2 prePlayerPos = Vector2.zero; // 대시 발동시 플레이어 위치값 저장용

    protected CapsuleCollider2D bodyCol; // 기존 콜라이더
    protected CapsuleCollider2D trigerCol; // 몸박용 트리거 콜라이더 설정

    protected LayerMask wallLayer; // 벽 감지용
    protected float rayDist = -1f; // 벽 감지 레이캐스트용 거리

    // 대시용 bool 변수
    protected bool bIsDash = false;
    public bool isPatternEnd = true;
    private bool isScreenPattern = false; // 스크린 패턴 중에는 몸박, TakeDamage, 콜라이더 비활성되게
    public bool IsScreenPattern 
    { 
        set 
        { 
            isScreenPattern = value;
            if (isScreenPattern)
            {
                bodyCol.enabled = false;
                trigerCol.enabled = false;
            }
            else
            {
                bodyCol.enabled = true;
                trigerCol.enabled = true;
            }

        } 
    }

    protected override void Awake()
    {
        base.Awake();
        baseSpeed = MoveSpeed; // 처음 인스펙터 창에서 세팅한 값으로 설정, 속도 변환에 사용
        myRoom = new BossRoom();
        bodyCol = GetComponent<CapsuleCollider2D>();
        // 몸 박용, 같은 크기의 캡슐 콜라이더 생성, 플레이어만 충돌판정 나도록
        trigerCol = gameObject.AddComponent<CapsuleCollider2D>();
        trigerCol.isTrigger = true;
        trigerCol.size = bodyCol.size;
        trigerCol.offset = bodyCol.offset;
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
        exclude |= 1 << 9; // 장애물도 제외
        trigerCol.excludeLayers = exclude; // 제외, 플레이어 빼고 다 제외

        bodyDamageTimer = bodyDamageTime; // 몸박당하자 마자 데미지 주기

        // 레이캐스트 벽 감지용
        wallLayer = LayerMask.GetMask("Wall");
        // 콜라이더 크기에 따른 레이캐스트 거리 구하기
        float halfWidth = bodyCol.size.x / 2f;
        float halfHeight = bodyCol.size.y / 2f;
        rayDist = Mathf.Sqrt(halfWidth * halfWidth + halfHeight * halfHeight);
        rayDist = Mathf.Ceil(rayDist); // 소수점 올림하여 거리 넉넉하게 잡기

    }

    protected override void Start()
    {
        base.Start();
        if (actor == null) return;

        actor = Instantiate(actor, this.transform);
    }

    protected override void Update()
    {
        if (BossState == EBossState.PowerOff) return; // 비활성화시 트리 실행 x
        //Debug.Log("Update");
        if (target.closestEnemy == null && BossState == EBossState.Active)
        {
            target.closestEnemy = this;
            this.SetHp();
        }
        behaviorTreeRoot.Evaluate(); // 트리 검사
        // 트리 검사하고 이동 확정된 다음 부모 실행
        base.Update(); // 애니메이션
        
        AddBodyDamageToTarget();
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!bIsDash) return; // 대시 중일때만 판단

        // 벽에 부딪혔을 때 법선 구하기
        //Vector2 wallNormal = collision.contacts[0].normal;
        // 위 방식으로 구하면 위아래로 겹칠때 에러 발생

        // 레이캐스트로 벽 탐지
        Vector2 startPos = (Vector2)transform.position + bodyCol.offset;
        RaycastHit2D hit = Physics2D.Raycast(startPos, moveDirection, rayDist, wallLayer); // 캡슐 크기에 따라 레이캐스트 거리 달라져야함
        Vector2 wallNormal = hit.normal;
        //Debug.DrawLine(startPos, startPos + moveDirection * rayDist, Color.gray, 1.0f);

        // 만약 법선과 moveDirection의 방향이 180도에 가깝다면 대시 중지
        float angle = Vector2.Angle(wallNormal, moveDirection);
        if (angle > 170f)
        {
            //Debug.Log($" 멈춤 각도: {angle}, 법선 백터: {wallNormal}, 레이 거리: {rayDist}");

            prePlayerPos = transform.position;
            return;
        }
        //else Debug.Log($" 진행 각도: {angle}, 법선 백터: {wallNormal}, 레이 거리: {rayDist}");

        // 기존 대시 벡터에서 수직 성분 제거 → 벽을 따라 미끄러지는 방향 계산
        Vector2 slideDirection = moveDirection - Vector2.Dot(moveDirection, wallNormal) * wallNormal;
        // 보스의 이동방향 갱신
        moveDirection = slideDirection.normalized;

        // 남은 거리 구하기
        float dist = Vector2.Distance(transform.position, prePlayerPos);
        // prePlayerPos 갱신
        prePlayerPos = (Vector2)transform.position + moveDirection * dist;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isScreenPattern) return;
        Debug.Log($"Trigger 몸박시작: {collision.gameObject.name}");
        bPlayerInBody = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isScreenPattern) return;

        Debug.Log($"Trigger 몸박종료: {collision.gameObject.name}");
        bPlayerInBody = false;
    }
    protected virtual void TakeDamage(int  damage)
    {
        if (isScreenPattern) return;
        base.TakeDamage(damage);
    }
    protected virtual void Init(Player _target)
    {
        target = _target;
    }
    protected abstract void SetDirection();
    protected abstract INode SetBehaviorTree();

    protected override void SetHp()
    {
        float hpScale = 1.0f + (target.killCount * 0.1f);
        MaxHp = (int)(MaxHp * hpScale);
        base.SetHp();
    }

    protected override void OnDead()
    {
        // 기본 몬스터 죽음
        base.OnDead();
        target.killCount++;
        target.closestEnemy = null;
        target.Levelup();
        // 해당 적 죽으면 방 문 열고, 
        GameManager.Instance.MapGenerater.OpenBossRoom(MyRoom.RoomWallIdx);
        // 카드 선택UI 키기
        UIManager.Instance.SetSelectCardUIActive();
        // 승리 BGM 추가
        AudioManager.Instance.PlayBGM(BGMType.Victory);

        //Destroy(this.gameObject); 파괴하면 여러 문제 생김
        gameObject.SetActive(false); // 일단 비활성화로
    }

    void AddBodyDamageToTarget()
    {
        if (!bPlayerInBody) return;

        bodyDamageTimer += Time.deltaTime;
        if (bodyDamageTimer >= bodyDamageTime)
        {
            bodyDamageTimer -= bodyDamageTime;
            //Debug.Log($"플레이어에게 몸박 데미지{bodyDamage}");
            ApplyDamage(target, bodyDamage, false, false);
        }
    }
    protected virtual INode.ENodeState Chase()
    {
        //Debug.Log("Chase");
        if (Vector3.Distance(target.transform.position, transform.position) <= chaseRange) // 범위 안이라면 쫓아가기
        {
            //Debug.Log("ChaseSuccess");
            moveDirection = (target.transform.position - transform.position).normalized;
            MoveSpeed = baseSpeed; // 가까워지면 기본 속도로
            return INode.ENodeState.Success;
        }
        else // 아니라면 대시
        {
            //Debug.Log("ChaseFailureAndDash");
            moveDirection = (target.transform.position - transform.position).normalized;
            // 변수 따로 빼기?**************
            MoveSpeed = target.GetComponent<Player>().MoveSpeed * dashMultiple; // 타겟 스피드의 n배로

            bIsDash = true;
            prePlayerPos = target.transform.position;
            return INode.ENodeState.Failure; // 이 다음에 행동 노드가 있다면 다른 걸 해야할 수도 있다. 지금은 상관 없다.
        }
    }
    protected virtual INode.ENodeState DaseToPlayerPos()
    {
        if (Vector3.Distance(prePlayerPos, transform.position) < 0.5f) // 거리가 일정 거리 미만이라면 위치 갱신 후 리턴 성공
        {
            transform.position = prePlayerPos;
            MoveSpeed = baseSpeed;
            moveDirection = Vector2.zero;
            bIsDash = false;

            return INode.ENodeState.Success;
        }
        return INode.ENodeState.Running;
    }
    protected virtual INode.ENodeState IsActive()
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
    protected virtual INode.ENodeState MoveToCenter()
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
    

    // 테스트 용
    public void KillBoss()
    {
        OnDead();
    }
    
}

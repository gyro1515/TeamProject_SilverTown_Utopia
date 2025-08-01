using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Entity
{
    [Header("보스 세팅")]
    [SerializeField] public Player target;
    [SerializeField] protected float attackRange = 3.0f;
    [SerializeField] protected float chaseRange = 10.0f;
    [SerializeField] protected PatternActor actor;
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

    public bool isPatternEnd = true;

    protected override void Awake()
    {
        base.Awake();
        baseSpeed = MoveSpeed; // 처음 인스펙터 창에서 세팅한 값으로 설정, 속도 변환에 사용
        myRoom = new BossRoom();
    }

    protected override void Start()
    {
        base.Start();
        actor = Instantiate(actor, this.transform);
    }

    protected override void Update()
    {
        if (BossState == EBossState.PowerOff) return; // 비활성화시 트리 실행 x
        //Debug.Log("Update");
        behaviorTreeRoot.Evaluate(); // 트리 검사
        // 트리 검사하고 이동 확정된 다음 부모 실행
        base.Update();
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
        base.OnDead();
        target.killCount++;
        Destroy(this.gameObject);
    }
}

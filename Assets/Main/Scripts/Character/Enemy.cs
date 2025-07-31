using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Entity
{
    [Header("보스 세팅")]
    [SerializeField] private GameObject target;
    [SerializeField] private float attackRange = 3.0f;
    [SerializeField] private float chaseRange = 10.0f;
    public enum EBossState // 보스 방 진입시 활성화, 탈출 시 비활성화(맵 중앙으로 돌아가기), 중앙으로 돌아가면 작동 안하도록(업데이트x)
    {
        Active, Deactivate, PowerOff
    }
    protected EBossState bossState = EBossState.Deactivate;
    public EBossState BossState { set { BossState = value; } get { return BossState; } }

    BossRoom myRoom = null; // 

    protected INode behaviorTreeRoot;

    protected virtual void Init(GameObject _target)
    {
        target = _target;
    }
    protected abstract void SetDirection();
    protected abstract void SetBehaviorTree();
}

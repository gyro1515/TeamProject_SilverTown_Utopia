using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBoss : MonoBehaviour
{
    Rigidbody2D _rig2d;
    Vector2 moveDir = Vector2.zero;
    public float monSpeed = 8.0f;
    float speed = 8.0f;

    INode root;
    public GameObject target;
    // 공격 사거리
    public float attackRange = 3.0f;
    public float chaseRange = 10.0f;
    bool isAttacking = false;
    float attackTimer = 0f;
    public float attackDuration = 1.0f; // 공격 모션 시간

    private void Awake()
    {
        root = SetBehaviorTree();
        _rig2d = GetComponent<Rigidbody2D>();
        speed = monSpeed;
    }
    private void Update()
    {
        root.Evaluate(); // 트리 검사
    }
    private void FixedUpdate()
    {
        _rig2d.velocity = moveDir * speed;

    }

    INode SetBehaviorTree()
    {
        ActionNode canAttack = new ActionNode(CanAttack); // 액션 노드는 함수 바인딩
        ActionNode attack = new ActionNode(Attack); // 액션 노드는 함수 바인딩
        ActionNode chase = new ActionNode(Chase);
        SequenceNode attackSequence = new SequenceNode(new List<INode>() { canAttack, attack });
        SelectorNode selector = new SelectorNode(new List<INode>() { attackSequence, chase}); // 루트에 해당

        // Selector 노드: 하나라도 성공하면 성공
        // Sequence 노드: 모두 성공해야 성공
        // 현재 BT
        // 루트:                                       selector 
        // 루트의 자식:                   attackSequence,     chase
        // attackSequence의 자식:     canAttack, Attack
        // 체크 순서 1. selector, 2. attackSequence, 3. canAttack, 4. Attack, 5. chase
        // 예시: 1 -> 2 -> 3 성공 -> 4 성공 -> 2 성공 -> Selector는 첫 번째 자식이 Success였으므로 트리 전체는 Success로 종료
        //       1 -> 2 -> 3 성공 -> 4 러닝 -> 2 러닝 -> Selector도 Running 상태로 종료 (다음 프레임에서 이어서 평가)
        //       1 -> 2 -> 3 실패 -> 5 성공 -> Selector는 두 번째 자식(chase)이 성공했으므로 트리 전체는 Success로 종료

        return selector;
    }
    INode.ENodeState Chase()
    {
        //Debug.Log("Chase");
        if (Vector3.Distance(target.transform.position, transform.position) <= chaseRange)
        {
            //Debug.Log("ChaseSuccess");
            moveDir = (target.transform.position - transform.position).normalized;
            //speed = target.GetComponent<TestPlayer>().speed * 2; // 타겟 스피드의 2배로
            speed = monSpeed;
            // 회전 하면서 쫓아가기
            /*Vector3 dir = target.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180f;*/
            //transform.rotation = Quaternion.Euler(0, 0, angle);

            return INode.ENodeState.Success;
        }
        else if (Vector3.Distance(target.transform.position, transform.position) <= chaseRange * 2.0f)
        {
            //Debug.Log("ChaseSuccess");
            moveDir = (target.transform.position - transform.position).normalized;
            //speed = target.GetComponent<TestPlayer>().speed * 2; // 타겟 스피드의 2배로

            // 회전 하면서 쫓아가기
            /*Vector3 dir = target.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180f;*/
            //transform.rotation = Quaternion.Euler(0, 0, angle);

            return INode.ENodeState.Success;
        }
        else
        {
            //Debug.Log("ChaseFailure");
            moveDir = Vector2.zero;
            speed = monSpeed;
            //return INode.ENodeState.Failure;
            return INode.ENodeState.Running; // 범위 밖이라고 판단되면 계속 Chase()부분만 실행가는 하도록
            // 쫓아가기 범위 밖이면 애초에 공격 범위 생각할 필요 없음
        }

    }
    INode.ENodeState CanAttack()
    {
        //Debug.Log("CanAttack");

        // 공격 범위 내 혹은 공격 중이라면 성공 판정
        if (Vector3.Distance(target.transform.position, transform.position) <= attackRange || isAttacking) 
        {
            //Debug.Log("CanAttackSuccess");
            moveDir = Vector2.zero;
            return INode.ENodeState.Success;
        }

        return INode.ENodeState.Failure;
    }
    INode.ENodeState Attack()
    {
        //Debug.Log("Attack");

        if (!isAttacking) // 공격 중이 아니라면 
        {
            //Debug.Log("공격 시작");
            isAttacking = true;
            attackTimer = attackDuration;

            // 이 시점에 애니메이션이나 이펙트 실행도 가능
            // animator.SetTrigger("Attack");
            // 회전 하면서 쫓아가기
            /*Vector3 dir = target.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);*/
            return INode.ENodeState.Running; // 진행 중 표시, 노드가 다른 곳으로 안가도록
        }
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            return INode.ENodeState.Running; // 아직 공격이 진행 중이라면 Running
        }

        // 공격 종료
        //Debug.Log("공격 종료");
        isAttacking = false;
        return INode.ENodeState.Success;
    }
}

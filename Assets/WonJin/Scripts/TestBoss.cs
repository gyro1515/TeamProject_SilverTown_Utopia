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
    // ���� ��Ÿ�
    public float attackRange = 3.0f;
    public float chaseRange = 10.0f;
    bool isAttacking = false;
    float attackTimer = 0f;
    public float attackDuration = 1.0f; // ���� ��� �ð�

    private void Awake()
    {
        root = SetBehaviorTree();
        _rig2d = GetComponent<Rigidbody2D>();
        speed = monSpeed;
    }
    private void Update()
    {
        root.Evaluate(); // Ʈ�� �˻�
    }
    private void FixedUpdate()
    {
        _rig2d.velocity = moveDir * speed;

    }

    INode SetBehaviorTree()
    {
        ActionNode canAttack = new ActionNode(CanAttack); // �׼� ���� �Լ� ���ε�
        ActionNode attack = new ActionNode(Attack); // �׼� ���� �Լ� ���ε�
        ActionNode chase = new ActionNode(Chase);
        SequenceNode attackSequence = new SequenceNode(new List<INode>() { canAttack, attack });
        SelectorNode selector = new SelectorNode(new List<INode>() { attackSequence, chase}); // ��Ʈ�� �ش�

        // Selector ���: �ϳ��� �����ϸ� ����
        // Sequence ���: ��� �����ؾ� ����
        // ���� BT
        // ��Ʈ:                                       selector 
        // ��Ʈ�� �ڽ�:                   attackSequence,     chase
        // attackSequence�� �ڽ�:     canAttack, Attack

        return selector;
    }
    INode.ENodeState Chase()
    {
        if (Vector3.Distance(target.transform.position, transform.position) <= chaseRange)
        {
            //Debug.Log("ChaseSuccess");
            moveDir = (target.transform.position - transform.position).normalized;
            //speed = target.GetComponent<TestPlayer>().speed * 2; // Ÿ�� ���ǵ��� 2���
            speed = monSpeed;
            // ȸ�� �ϸ鼭 �Ѿư���
            Vector3 dir = target.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            return INode.ENodeState.Success;
        }
        else if (Vector3.Distance(target.transform.position, transform.position) <= chaseRange * 1.5f)
        {
            //Debug.Log("ChaseSuccess");
            moveDir = (target.transform.position - transform.position).normalized;
            speed = target.GetComponent<TestPlayer>().speed * 2; // Ÿ�� ���ǵ��� 2���

            // ȸ�� �ϸ鼭 �Ѿư���
            Vector3 dir = target.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            return INode.ENodeState.Success;
        }
        else
        {
            //Debug.Log("ChaseFailure");
            moveDir = Vector2.zero;
            speed = monSpeed;
            return INode.ENodeState.Failure;
        }

    }
    INode.ENodeState CanAttack()
    {
        // ���� ���� �� Ȥ�� ���� ���̶�� ���� ����
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
        if (!isAttacking) // ���� ���� �ƴ϶�� 
        {
            Debug.Log("���� ����");
            isAttacking = true;
            attackTimer = attackDuration;

            // �� ������ �ִϸ��̼��̳� ����Ʈ ���൵ ����
            // animator.SetTrigger("Attack");
            // ȸ�� �ϸ鼭 �Ѿư���
            Vector3 dir = target.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            return INode.ENodeState.Running; // ���� �� ǥ��, ��尡 �ٸ� ������ �Ȱ�����
        }
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            return INode.ENodeState.Running; // ���� ������ ���� ���̶�� Running
        }

        // ���� ����
        Debug.Log("���� ����");
        isAttacking = false;
        return INode.ENodeState.Success;
    }
}

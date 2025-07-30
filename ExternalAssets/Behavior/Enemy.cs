using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ���� ���۰� �ൿ Ʈ���� ����ϴ� Ŭ����
public class Enemy : MonoBehaviour
{
    public Transform player; // ������ �÷��̾�
    public float moveSpeed = 2f; // �̵� �ӵ�
    public EnemyData data; // ���� ����

    private Node rootNode; // ��Ʈ ��� (Selector)
    private float[] lastUsedTime; // ���Ϻ� ������ ��� �ð� ���

    void Start()
    {
        rootNode = CreateBehaviorTree(); // �ൿ Ʈ�� ����
        lastUsedTime = new float[data.patterns.Count]; // ���� ����ŭ �ʱ�ȭ
    }

    void Update()
    {
        rootNode.Evaluate(); // �� �����Ӹ��� Ʈ�� �� ����
    }

    private Node CreateBehaviorTree()
    {
        var patternNodes = new List<Node>(); // ���� ��� ����Ʈ ����
        for (int i = 0; i < data.patterns.Count; i++)
        {
            patternNodes.Add(new PatternNode(this, i)); // ������ ���� ��� �߰�
        }
        return new Selector(patternNodes); // Selector�� ���� ��Ʈ ��� ����
    }

    public PatternInfo GetPatternInfo(int index)
    {
        if (index < 0 || index >= data.patterns.Count) return null; // ��ȿ���� ������ null
        return data.patterns[index]; // �ش� �ε����� ���� ��ȯ
    }

    public bool CanUsePattern(PatternInfo pattern)
    {
        int index = data.patterns.IndexOf(pattern); // ������ �ε��� ã��
        float dist = DistanceToPlayer(); // �÷��̾�� �Ÿ� ���

        if (dist > pattern.triggerDistance) return false; // �Ÿ��� �ָ� �ߵ� �Ұ�
        if (Time.time - lastUsedTime[index] < pattern.cooldown) return false; // ��Ÿ�� ������

        return true; // ���� ���� �� ���� ����
    }

    public void ExecutePattern(PatternInfo pattern)
    {
        int index = data.patterns.IndexOf(pattern);
        lastUsedTime[index] = Time.time; // ������ ��� �ð� ����

        if (!string.IsNullOrEmpty(pattern.animationTrigger))
        {
            Debug.Log($"{gameObject.name} �� ���� {index + 1} �ߵ�! (�ִ�: {pattern.animationTrigger})");
        }
        else
        {
            Debug.Log($"{gameObject.name} �� ���� {index + 1} �ߵ�!");
        }

        // �� �κп� ���� ������, ���� ���� �� ���� ����
    }

    public float DistanceToPlayer()
    {
        if (player == null) return float.MaxValue; // �÷��̾� ������ �ִ밪 ��ȯ
        return Vector3.Distance(transform.position, player.position); // �Ÿ� ���
    }
}
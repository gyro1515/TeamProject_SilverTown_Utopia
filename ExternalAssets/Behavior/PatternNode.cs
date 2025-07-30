using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� 1~4�� ó���� ���� ���� ���
public class PatternNode : Node
{
    private Enemy enemy; // �� ĳ���� ����
    private int patternIndex; // ����� ���� ��ȣ

    public PatternNode(Enemy enemy, int patternIndex)
    {
        this.enemy = enemy;
        this.patternIndex = patternIndex;
    }

    public override NodeState Evaluate()
    {
        var pattern = enemy.GetPatternInfo(patternIndex); // �ش� �ε����� ���� ���� ��������
        if (pattern == null || !enemy.CanUsePattern(pattern)) // ���� ������ �� ����
            return NodeState.Failure;

        enemy.ExecutePattern(pattern); // ���� ����
        return NodeState.Success; // ���� ��ȯ
    }
}
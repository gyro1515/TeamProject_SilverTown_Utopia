using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� �ڽ� ��� �� �ϳ��� �����ϸ� �������� �Ǵ��ϴ� ���� ���
public class Selector : Node
{
    protected List<Node> nodes = new List<Node>(); // �ڽ� ��� ����Ʈ

    public Selector(List<Node> nodes)
    {
        this.nodes = nodes; // �����ڿ��� �ڽ� ��� ����
    }

    public override NodeState Evaluate()
    {
        foreach (var node in nodes)
        {
            var result = node.Evaluate(); // �ڽ� ��带 ���������� ��
            if (result == NodeState.Success || result == NodeState.Running)
                return result; // �����ϰų� ���� ���̸� �� ����� ��ȯ
        }
        return NodeState.Failure; // ��� ���� �� ���� ��ȯ
    }
}
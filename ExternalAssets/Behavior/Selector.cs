using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 여러 자식 노드 중 하나라도 성공하면 성공으로 판단하는 선택 노드
public class Selector : Node
{
    protected List<Node> nodes = new List<Node>(); // 자식 노드 리스트

    public Selector(List<Node> nodes)
    {
        this.nodes = nodes; // 생성자에서 자식 노드 지정
    }

    public override NodeState Evaluate()
    {
        foreach (var node in nodes)
        {
            var result = node.Evaluate(); // 자식 노드를 순차적으로 평가
            if (result == NodeState.Success || result == NodeState.Running)
                return result; // 성공하거나 실행 중이면 그 결과를 반환
        }
        return NodeState.Failure; // 모두 실패 시 실패 반환
    }
}
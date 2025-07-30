using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class SelectorNode : INode
{
    private List<INode> childrens;
    // �����ϋ� �ٷ� �ش� �ε����� ������ ĳ�� ���� ����
    private int currentIndex = 0;

    public SelectorNode(List<INode> nodes)
    {
        childrens = nodes;
    }

    public INode.ENodeState Evaluate()
    {
        if (childrens == null)
            return INode.ENodeState.Failure;

        while (currentIndex < childrens.Count)
        {
            INode.ENodeState result = childrens[currentIndex].Evaluate();

            switch (result)
            {
                case INode.ENodeState.Success:
                    currentIndex = 0;
                    return INode.ENodeState.Success;

                case INode.ENodeState.Running:
                    return INode.ENodeState.Running;

                case INode.ENodeState.Failure:
                    currentIndex++;
                    continue;
            }
        }

        currentIndex = 0;
        return INode.ENodeState.Failure;
    }
}

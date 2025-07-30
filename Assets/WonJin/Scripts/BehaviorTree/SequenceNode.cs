using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : INode
{
    private List<INode> childrens;
    // �����ϋ� �ٷ� �ش� �ε����� ������ ĳ�� ���� ����
    private int currentIndex = 0;

    public SequenceNode(List<INode> childs)
    {
        childrens = childs;
    }

    public INode.ENodeState Evaluate()
    {
        Debug.Log("SequenceNode");

        if (childrens == null || childrens.Count == 0)
            return INode.ENodeState.Failure;

        while (currentIndex < childrens.Count)
        {
            INode.ENodeState result = childrens[currentIndex].Evaluate();

            switch (result)
            {
                case INode.ENodeState.Success:
                    currentIndex++; // ���� �ڽ�����
                    continue;
                case INode.ENodeState.Running:
                    return INode.ENodeState.Running; // ���� �ڽĿ��� ����

                case INode.ENodeState.Failure:
                    currentIndex = 0; // ���� �� ����
                    return INode.ENodeState.Failure;
            }
        }

        // ��� �ڽ� ���� �� ������ ����
        currentIndex = 0; // ���� �� ����
        return INode.ENodeState.Success;
    }


}

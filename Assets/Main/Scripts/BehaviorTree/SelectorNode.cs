using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
//using static Unity.VisualScripting.Metadata;

public class SelectorNode : INode
{
    private List<INode> childrens;
    // 러닝일떄 바로 해당 인덱스로 가도록 캐시 구조 적용
    private int currentIndex = 0;

    public SelectorNode(List<INode> nodes)
    {
        childrens = nodes;
    }

    public INode.ENodeState Evaluate()
    {
        //Debug.Log("SelectorNode");

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

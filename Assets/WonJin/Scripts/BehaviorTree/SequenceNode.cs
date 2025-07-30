using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : INode
{
    private List<INode> childrens;


    public SequenceNode(List<INode> childs)
    {
        childrens = childs;
    }

    public INode.ENodeState Evaluate()
    {
        if (childrens == null || childrens.Count == 0)
            return INode.ENodeState.Failure;

        foreach (var child in childrens)
        {
            switch (child.Evaluate())
            {
                case INode.ENodeState.Running:
                    return INode.ENodeState.Running;
                case INode.ENodeState.Success:
                    continue;
                case INode.ENodeState.Failure:
                    return INode.ENodeState.Failure;
            }
        }

        return INode.ENodeState.Success;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class SelectorNode : INode
{
    private List<INode> childrens;
    public SelectorNode(List<INode> nodes)
    {
        childrens = nodes;
    }

    public INode.ENodeState Evaluate()
    {
        if (childrens == null)
            return INode.ENodeState.Failure;

        foreach (var child in childrens)
        {
            switch (child.Evaluate())
            {
                case INode.ENodeState.Running:
                    return INode.ENodeState.Running;
                case INode.ENodeState.Success:
                    return INode.ENodeState.Success;
            }
        }

        return INode.ENodeState.Failure;
    }
}

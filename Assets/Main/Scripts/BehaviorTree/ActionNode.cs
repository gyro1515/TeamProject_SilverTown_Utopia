using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : INode
{
    Func<INode.ENodeState> onUpdate = null;
    public ActionNode(Func<INode.ENodeState> _onUpdate)
    {
        onUpdate = _onUpdate;
    }

    public INode.ENodeState Evaluate() => onUpdate?.Invoke() ?? INode.ENodeState.Failure;
}

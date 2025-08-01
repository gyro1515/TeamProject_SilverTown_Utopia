using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownNode : INode
{
    private INode child;
    private float cooldownTime;
    private float cooldownTimer = 0f;

    public CooldownNode(INode _child, float _cooldownTime)
    {
        child = _child;
        cooldownTime = _cooldownTime;
    }

    public INode.ENodeState Evaluate()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            return INode.ENodeState.Failure; // 아직 쿨다운 중이므로 실패
        }

        INode.ENodeState result = child.Evaluate();

        if (result == INode.ENodeState.Success)
        {
            cooldownTimer = cooldownTime; // 성공 시 쿨타임 발동
        }

        return result;
    }
}
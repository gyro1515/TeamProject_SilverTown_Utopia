using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitNode : INode
{
    private float waitTime;
    private float timer = 0f;
    private bool isWaiting = false;

    public WaitNode(float _waitTime)
    {
        waitTime = _waitTime;
    }

    public INode.ENodeState Evaluate()
    {
        if (!isWaiting)
        {
            timer = waitTime;
            isWaiting = true;
        }

        timer -= Time.deltaTime;

        if (timer > 0f)
        {
            //Debug.Log("기다리는 중");
            return INode.ENodeState.Running;
        }

        isWaiting = false;
        return INode.ENodeState.Success;
    }
}

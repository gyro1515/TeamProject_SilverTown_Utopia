using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : INode
{
    private List<INode> childrens;
    // 러닝일떄 바로 해당 인덱스로 가도록 캐시 구조 적용
    private int currentIndex = 0;

    public SequenceNode(List<INode> childs)
    {
        childrens = childs;
    }

    public INode.ENodeState Evaluate()
    {
        //Debug.Log("SequenceNode");

        if (childrens == null || childrens.Count == 0)
            return INode.ENodeState.Failure;

        while (currentIndex < childrens.Count)
        {
            INode.ENodeState result = childrens[currentIndex].Evaluate();

            switch (result)
            {
                case INode.ENodeState.Success:
                    currentIndex++; // 다음 자식으로
                    continue;
                case INode.ENodeState.Running:
                    return INode.ENodeState.Running; // 현재 자식에서 멈춤

                case INode.ENodeState.Failure:
                    currentIndex = 0; // 실패 시 리셋
                    return INode.ENodeState.Failure;
            }
        }

        // 모든 자식 성공 → 시퀀스 성공
        currentIndex = 0; // 종료 후 리셋
        return INode.ENodeState.Success;
    }


}

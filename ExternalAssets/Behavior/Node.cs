using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 노드가 반환할 수 있는 상태 정의 (실행 중, 성공, 실패)
public enum NodeState { Running, Success, Failure };

// 모든 노드가 상속받는 추상 클래스
public abstract class Node
{
    public abstract NodeState Evaluate(); // 노드 평가 실행
}
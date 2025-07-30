using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��尡 ��ȯ�� �� �ִ� ���� ���� (���� ��, ����, ����)
public enum NodeState { Running, Success, Failure };

// ��� ��尡 ��ӹ޴� �߻� Ŭ����
public abstract class Node
{
    public abstract NodeState Evaluate(); // ��� �� ����
}
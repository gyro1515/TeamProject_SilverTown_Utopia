using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ϳ��� ���� ���� ������ ��� Ŭ����
[System.Serializable]
public class PatternInfo
{
    public float triggerDistance; // �ߵ� �Ÿ�
    public float damage; // ���ط�
    public float cooldown; // ��Ÿ��
    public string animationTrigger; // �ִϸ��̼� Ʈ���� �̸�
}

// ���� ���� ��� ���ϵ��� ��� Ŭ����
public class EnemyData : MonoBehaviour
{
    public List<PatternInfo> patterns = new List<PatternInfo>(); // ���� ����Ʈ
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� �κ��� �������� �����ؾ� ������...?
public class Room : MonoBehaviour
{
    RectInt room;
    // ���߿� Enemy ��������� �ٲ����
    GameObject boss;
    TestBoss testBoss;

    public void Init(RectInt _room, GameObject _boss)
    {
        room = _room;
        boss = _boss;
        testBoss = boss.GetComponent<TestBoss>();
    }

    private void Update()
    {
        if (testBoss.BossState == TestBoss.EBossState.Active) return; // Ȱ��ȭ�Ǿ� �ִٸ� ����

        // ��Ȱ���Ǿ� �ִٸ� �� �߾�����
        

    }
}

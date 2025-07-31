using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이 부분을 보스에서 구현해야 할지도...?
public class Room : MonoBehaviour
{
    RectInt room;
    // 나중에 Enemy 만들어지면 바꿔야함
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
        if (testBoss.BossState == TestBoss.EBossState.Active) return; // 활성화되어 있다면 리턴

        // 비활성되어 있다면 맵 중앙으로
        

    }
}

using System.Collections;
using System.Collections.Generic;
//using UnityEditor.UIElements;
using UnityEngine;

public class RoadTriger : MonoBehaviour
{
    private bool bIsOnRoad = false;
    private MapGenerater mapGenerater = null;
    public void Init(MapGenerater _mapGenerater)
    {
        mapGenerater = _mapGenerater;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        mapGenerater.IsMoveToRoom = false; // 복도에 진입했다면 방탐색 중지

        // 복도 진입
        //Debug.Log("복도 진입");
        bIsOnRoad = true;
        // 보스 전부 비활성화
        mapGenerater.SetBossDeActive();
        AudioManager.Instance.PlayBGM(BGMType.Feild);

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!bIsOnRoad) return;
        // 먼저 콜리전이 플레이어인지 체크
        if (!collision.CompareTag("Player")) return;

        mapGenerater.IsMoveToRoom = true; // 복도에서 나왔다면 방 탐색 시작
        // 방을 찼았다면 자동으로 탐색 중지

    }
}

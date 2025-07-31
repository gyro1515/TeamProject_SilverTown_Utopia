using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class RoadTriger : MonoBehaviour
{
    private bool bIsOnRoad = false;
    private MapGenerater mapGenerater = null;
    private GameObject player = null;
    TestCam testCam = null;
    public void Init(MapGenerater _mapGenerater, GameObject _player)
    {
        mapGenerater = _mapGenerater;
        player = _player;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // 복도 진입
        //Debug.Log("복도 진입");
        bIsOnRoad = true;
        if(testCam == null) testCam = FindObjectOfType<TestCam>();

        // 보스 전부 비활성화
        mapGenerater.SetBossDeActive();

        testCam.CamState = TestCam.ECameraState.ChageToRoad;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!bIsOnRoad) return;
        // 먼저 콜리전이 플레이어인지 체크
        if (!collision.CompareTag("Player")) return;

        // 이동한 맵 가져오기
        RectInt tmp = mapGenerater.GetRoomByPos(collision.transform.position);
        // 카메라 제한을 이동한 맵에 맞추기
        if (testCam == null) testCam = FindObjectOfType<TestCam>();
        testCam.SetMap(tmp);

    }
}

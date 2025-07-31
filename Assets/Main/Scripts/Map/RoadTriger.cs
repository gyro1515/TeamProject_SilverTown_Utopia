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

        // ���� ����
        //Debug.Log("���� ����");
        bIsOnRoad = true;
        if(testCam == null) testCam = FindObjectOfType<TestCam>();

        // ���� ���� ��Ȱ��ȭ
        mapGenerater.SetBossDeActive();

        testCam.CamState = TestCam.ECameraState.ChageToRoad;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!bIsOnRoad) return;
        // ���� �ݸ����� �÷��̾����� üũ
        if (!collision.CompareTag("Player")) return;

        // �̵��� �� ��������
        RectInt tmp = mapGenerater.GetRoomByPos(collision.transform.position);
        // ī�޶� ������ �̵��� �ʿ� ���߱�
        if (testCam == null) testCam = FindObjectOfType<TestCam>();
        testCam.SetMap(tmp);

    }
}

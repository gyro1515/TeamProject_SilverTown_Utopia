using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCam : MonoBehaviour
{
    public enum ECameraState
    {
        InRoom, InRoad, ChageToRoad, ChangeToRoom
    }
    ECameraState camState;
    public ECameraState CamState 
    {
        get { return camState; }
        set
        {
            camState = value;
            switch (camState)
            {
                case ECameraState.ChageToRoad:
                case ECameraState.ChangeToRoom:
                    isChanging = true;
                    break;
                default:
                    break;
            }
        }
    }
    bool isChanging = false;

    [SerializeField] GameObject target;
    Camera cam = null;
    RectInt map;
    [SerializeField] int roomPadding = 1;
    float changingTime = 0.5f;
    float changingTimer = 0.0f;

    private void Awake()
    {
        cam = Camera.main;
    }
    private void Update()
    {
        //if (CamState == ECameraState.ChangeToRoom || CamState == ECameraState.ChageToRoad) return;
        if (!isChanging) return;

        Vector3 tarPos = target.transform.position;
        tarPos.z = cam.transform.position.z;
        switch (CamState)
        {
            case ECameraState.ChangeToRoom:
                changingTimer += Time.fixedDeltaTime;
                if (changingTimer >= changingTime)
                {
                    changingTimer = 0.0f;
                    CamState = ECameraState.InRoom;
                    isChanging = false;
                }
                
                tarPos = Vector3.Lerp(cam.transform.position, CameraClamp(tarPos), 0.3f);
                cam.transform.position = tarPos;
                break;
            case ECameraState.ChageToRoad:
                changingTimer += Time.fixedDeltaTime;
                if (changingTimer >= changingTime)
                {
                    changingTimer = 0.0f;
                    CamState = ECameraState.InRoad;
                    isChanging = false;
                }
                tarPos = Vector3.Lerp(cam.transform.position, tarPos, 0.3f);
                cam.transform.position = tarPos;
                break;
        }
    }
    private void FixedUpdate()
    {
        Vector3 tarPos = target.transform.position;
        tarPos.z = cam.transform.position.z;
        switch (CamState)
        {
            case ECameraState.InRoom:
                cam.transform.position = CameraClamp(tarPos);
                break;
            case ECameraState.InRoad:
                cam.transform.position = tarPos;
                break;
            default:
                break;
        }
        
    }
    public Vector3 CameraClamp(Vector3 pos)
    {
        Vector3 camPos = pos;
        float camHalfY = cam.orthographicSize;   // Y축 방향 절반
        float camHalfX = camHalfY * cam.aspect; // X축 방향 절반 = orthographicSize * 화면비(16:9 = 1.7777)

        // 카메라가 볼 수 있는 영역 고려해서 클램핑
        float minX = map.min.x + camHalfX;
        float maxX = map.max.x - camHalfX; 
        float minY = map.min.y + camHalfY;
        float maxY = map.max.y - camHalfY;


        camPos.x = Mathf.Clamp(camPos.x, minX, maxX);
        camPos.y = Mathf.Clamp(camPos.y, minY, maxY);
        return camPos;
    }
    public void SetMap(RectInt _map)
    {
        RectInt expanded = new RectInt(_map.xMin - roomPadding, _map.yMin - roomPadding, _map.width + roomPadding * 2, _map.height + roomPadding * 2);
        map = expanded;
        CamState = TestCam.ECameraState.ChangeToRoom;
    }
}

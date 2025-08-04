using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이 부분을 보스에서 구현해야 할지도...?
public class BossRoom
{
    RectInt room;
    public RectInt Room { get { return room; } set { room = value; } }
    int roomIdx = -1;
    public int RoomIdx { get { return roomIdx; } set { roomIdx = value; } }
    int roomWallIdx = -1;
    public int RoomWallIdx { get { return roomWallIdx; } set { roomWallIdx = value; } }

    public BossRoom() { }
    public BossRoom(RectInt _room, int _roomIdx)
    {
        Room = _room;
        RoomIdx = _roomIdx;
    }
}

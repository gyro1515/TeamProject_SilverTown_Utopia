using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� �κ��� �������� �����ؾ� ������...?
public class BossRoom
{
    RectInt room;
    public RectInt Room { get { return room; } set { room = value; } }
    int roomIdx = -1;
    public int RoomIdx { get { return roomIdx; } set { roomIdx = value; } }

    public BossRoom(RectInt _room, int _roomIdx)
    {
        Room = _room;
        RoomIdx = _roomIdx;
    }
}

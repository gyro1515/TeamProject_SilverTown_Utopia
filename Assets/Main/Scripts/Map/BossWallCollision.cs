using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BossWallCollision : MonoBehaviour
{
    bool isBossRoom = false;
    bool isInBossRoom = false;
    int roomIdx = -1;
    int bossWallIdx = -1;

    CompositeCollider2D col = null;
    Tilemap map = null;
    private void Awake()
    {
        col = GetComponent<CompositeCollider2D>();
        map = gameObject.GetComponent<Tilemap>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isBossRoom || isInBossRoom) return;

        col.isTrigger = true;
        Color color = map.color;
        color.a = 0;
        map.color = color;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isBossRoom) return;
        col.isTrigger = false;
        Color color = map.color;
        color.a = 1;
        map.color = color;
    }

    public void Init(bool _isBossRoom, int _roomIdx, int _bossWallIdx)
    {
        isBossRoom = _isBossRoom;
        roomIdx = _roomIdx;
        bossWallIdx = _bossWallIdx;
    }

    public void SetIsInBossRoom(bool active)
    {
        isInBossRoom = active;
        if(active)
        {
            col.isTrigger = false;
            Color color = map.color;
            color.a = 1;
            map.color = color;
        }
    }
}

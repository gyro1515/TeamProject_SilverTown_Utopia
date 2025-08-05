using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("관리할 것")]
    [SerializeField] MapGenerater mapGenerater;
    public MapGenerater MapGenerater { get { return mapGenerater; } }
    Player player;
    public Player Player { get { return player; } }
    public const bool DEBUGMODE = false;

    protected override void Awake()
    {
        base.Awake();
        player = mapGenerater.PlayerGO.GetComponent<Player>();
    }
}

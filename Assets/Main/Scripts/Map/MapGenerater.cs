using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Windows;
using static UnityEditor.PlayerSettings;

public class MapGenerater : MonoBehaviour
{
    struct Map
    {
        RectInt map;
        int mapIdx;
    }
    private int bossRoomIdx;

    [Header("Map Settings")]
    public int totalCandidates = 50;
    public int desiredRoomCount = 20;
    public Vector2Int roomMinSize = new Vector2Int(6, 6);
    public Vector2Int roomMaxSize = new Vector2Int(12, 12);
    public int roomSpacing = 3;

    [Header("Tilemap Settings")]
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tilemap wallTilemap;
    [SerializeField] Tilemap bossWallMap;
    [SerializeField] Tilemap roadTriger;
    [SerializeField] TileBase floorTile;
    [SerializeField] TileBase wallTile;
    [SerializeField] TileBase bossWallTile; // 적과 보스 타일 구분용
    [SerializeField] TileBase enemyWallTile; // 적과 보스 타일 구분용

    [Header("Character Setting")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject boss;

    [Header("DrawLine Setting")]
    [SerializeField] float time = 4.0f;
    [Header("ObstacleObjcet 세팅")]
    [SerializeField] List<GameObject> obstacles = new List<GameObject>();
    List<GameObject> spawnObstacles = new List<GameObject>();

    [SerializeField] private int spawnObstacleCount = 20; // 용규 추가


    private List<RectInt> candidateRooms = new List<RectInt>();
    private List<RectInt> finalRooms = new List<RectInt>();
    public List<RectInt> FinalRooms { get { return finalRooms; } private set { } }
    private List<Vector2Int> roomCenters = new List<Vector2Int>();
    private HashSet<(int, int)> connections = new HashSet<(int, int)>();
    private Dictionary<int, List<int>> connectionMap = new Dictionary<int, List<int>>();
    // 보스룸 체크용, 몬스터가 많아진다면 방 클래스 만들어야...?
    

    private void Awake()
    {
        roadTriger.GetComponent<RoadTriger>().Init(this, player);

    }
    void Start() => GenerateMap();

    void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.E))
        {
            GenerateMap();
        }
        else if(UnityEngine.Input.GetKeyDown(KeyCode.R))
        {
            // 보스 방 진입 시 호출되는 함수, 추후 함수 다른 곳으로 이동해야 함
            // 보스 방의 벽 비활성화
            OpenBossRoom();
        }
    }

    void GenerateMap()
    {
        tilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        bossWallMap.ClearAllTiles();
        roadTriger.ClearAllTiles();
        candidateRooms.Clear();
        finalRooms.Clear();
        roomCenters.Clear();
        connections.Clear();
        connectionMap.Clear();
        foreach(var obs in spawnObstacles)
        {
            Destroy(obs);
        }
        spawnObstacles.Clear();

        GenerateInitialRooms();
        ResolveRoomCollisions();
        FilterAndSelectRooms();
        ConnectRooms();
        DrawRooms();

        DrawCorridors();
        DrawWalls();

        SetCharacter();
        SetObstacles();

        tilemap.CompressBounds(); // 타일 맵 크기 최적화
        wallTilemap.CompressBounds(); // 타일 맵 크기 최적화
        bossWallMap.CompressBounds(); // 타일 맵 크기 최적화
        roadTriger.CompressBounds(); // 타일 맵 크기 최적화
    }

    void GenerateInitialRooms()
    {
        for (int i = 0; i < totalCandidates; i++)
        {
            int w = Random.Range(roomMinSize.x, roomMaxSize.x);
            int h = Random.Range(roomMinSize.y, roomMaxSize.y);
            int x = Random.Range(-5 - w / 2, 5 - w / 2 + 1); // 중심 맞추기
            int y = Random.Range(-5 - h / 2, 5 - h / 2 + 1); // 중심 맞추기
            /*int x = -w / 2; // 중심 맞추기
            int y = -h / 2; // 중심 맞추기*/
            candidateRooms.Add(new RectInt(x, y, w, h));
        }
    }

    void ResolveRoomCollisions()
    {
        // 한 네모를 기준으로 나머지를 중심 밖으로 밀어내기
        for (int i = 0; i < candidateRooms.Count; i++)
        {
            RectInt a = candidateRooms[i];
            Vector2 centerA = a.center;

            bool moved = true;
            while (moved)
            {
                moved = false;
                for (int j = 0; j < candidateRooms.Count; j++)
                {
                    if (i == j) continue;
                    RectInt b = candidateRooms[j];
                    RectInt expandedB = new RectInt(b.xMin - roomSpacing, b.yMin - roomSpacing, b.width + roomSpacing * 2, b.height + roomSpacing * 2);
                    if(a.Overlaps(expandedB))
                    {
                        moved = true;
                        Vector2 centerB = b.center;
                        Vector2 dir = (centerB - Vector2.zero).normalized; 
                        
                        if (dir == Vector2.zero)
                            dir = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2)).normalized; // 같은 중심이라면 아무 방향으로
                                                                                                    // 상하 좌우로만 움직이기
                        b.position += Vector2Int.RoundToInt(dir); // 반올림해서 Vector2Int으로 변환
                        candidateRooms[j] = b;
                    }
                }
            }
        }
    }

    void FilterAndSelectRooms()
    {
        List<RectInt> largeEnough = new List<RectInt>(candidateRooms);
        // 사이즈 큰걸로 선별
        largeEnough.Sort((a, b) => b.size.magnitude.CompareTo(a.size.magnitude));
        finalRooms = largeEnough.GetRange(0, Mathf.Min(desiredRoomCount, largeEnough.Count));
        // 작은 것부터 큰 걸로 정렬
        finalRooms.Sort((a, b) => a.size.magnitude.CompareTo(b.size.magnitude));

        for (int i = 0; i < finalRooms.Count; i++)
        {
            //Vector2Int center = new Vector2Int((int)finalRooms[i].center.x, (int)finalRooms[i].center.y);
            Vector2Int center = Vector2Int.RoundToInt(finalRooms[i].center);
            roomCenters.Add(center);
            connectionMap[i] = new List<int>();
        }
    }

    void ConnectRooms()
    {
        int roomCount = roomCenters.Count;
        bool[] visited = new bool[roomCount];
        visited[0] = true;

        for (int edgeCount = 0; edgeCount < roomCount - 1; edgeCount++)
        {
            float minDist = float.MaxValue;
            int from = -1, to = -1;

            for (int i = 0; i < roomCount; i++)
            {
                if (!visited[i]) continue;
                for (int j = 0; j < roomCount; j++)
                {
                    if (visited[j]) continue;
                    float dist = Vector2Int.Distance(roomCenters[i], roomCenters[j]);
                    //if (dist < minDist && CanAddConnection(i, j)) // 최단거리이며, 연결 안된 룸인지 체크
                    if (dist < minDist) // 최단거리 룸
                    {
                        minDist = dist;
                        from = i;
                        to = j;
                    }
                }
            }

            if (from != -1 && to != -1)
            {
                AddConnection(from, to);
                visited[to] = true;
                Debug.DrawLine((Vector2)roomCenters[from], (Vector2)roomCenters[to],Color.red, time);
            }
        }
    }

    void AddConnection(int i, int j)
    {
        int a = Mathf.Min(i, j);
        int b = Mathf.Max(i, j);
        var conn = (a, b);
        if (!connections.Contains(conn))
        {
            connections.Add(conn);
            connectionMap[a].Add(b);
            connectionMap[b].Add(a);
        }
    }
    void DrawRooms()
    {
        foreach (var room in finalRooms)
        {
            for (int x = room.xMin; x < room.xMax; x++)
            {
                for (int y = room.yMin; y < room.yMax; y++)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
                }
            }
        }
    }
    void DrawCorridors()
    {
        foreach (var (i, j) in connections)
        {
            Vector2Int a = roomCenters[i];
            Vector2Int b = roomCenters[j];

            if (a.x == b.x || a.y == b.y)
            {
                DrawWideCorridor(a, b);
            }
            else
            {
                Vector2Int mid = new Vector2Int(b.x, a.y);
                //DrawWideCorridor(a, mid);
                //DrawWideCorridor(mid, b);
                
                DrawWideCorridor(a, mid);

                // 먼저 방향 구하기
                Vector2Int dir1 = new Vector2Int(
                    Mathf.Clamp(mid.x - a.x, -1, 1),
                    Mathf.Clamp(mid.y - a.y, -1, 1));
                Vector2Int dir2 = new Vector2Int(
                    Mathf.Clamp(b.x - mid.x, -1, 1),
                    Mathf.Clamp(b.y - mid.y, -1, 1));
                // 우하(→↓) 또는 좌상(←↑)일 경우 한 칸 보정
                if (dir1 == Vector2Int.right && dir2 == Vector2Int.down)
                {
                    PlaceCorridorTiles(mid + Vector2Int.right, dir1); // mid 지점에 추가 한 칸
                }
                else if (dir1 == Vector2Int.left && dir2 == Vector2Int.up)
                {
                    PlaceCorridorTiles(mid + Vector2Int.left, dir1); // mid 지점에 추가 한 칸
                }

                DrawWideCorridor(mid, b);
            }
        }
    }

    void DrawWideCorridor(Vector2Int from, Vector2Int to)
    {
        if (from == to) return;
        
        Vector2Int dir = new Vector2Int(
            Mathf.Clamp(to.x - from.x, -1, 1),
            Mathf.Clamp(to.y - from.y, -1, 1));

        Vector2Int current = from;
        while (current != to)
        {
            PlaceCorridorTiles(current, dir);
            current += dir;
        }
        PlaceCorridorTiles(current, dir);

    }

    void PlaceCorridorTiles(Vector2Int pos, Vector2Int dir)
    {
        Vector2Int perp = new Vector2Int(-dir.y, dir.x);
        for (int i = 0; i <= 1; i++)
        {
            Vector2Int tilePos = pos + perp * i;
            if (tilemap.GetTile((Vector3Int)tilePos) == floorTile) continue; // 이미 깔렸다면 다음
            
            tilemap.SetTile(new Vector3Int(tilePos.x, tilePos.y, 0), floorTile);

            // 문에 해당 하는 부분은 트리거 설정 안하기
            bool bDraw = true;
            foreach (var room in finalRooms)
            {
                RectInt expanded = new RectInt(room.xMin - 1, room.yMin - 1, room.width + 1 * 2, room.height + 1 * 2);

                if (expanded.Contains(tilePos)) // 해당 좌표가 맵 안에 있다면 안그리기
                {
                    //Debug.Log($"Skip: {tilePos}");
                    bDraw = false;
                    break;
                }
            }
            if (!bDraw) continue;
            roadTriger.SetTile(new Vector3Int(tilePos.x, tilePos.y, 0), floorTile);
            //Debug.Log($"Draw: {tilePos}");
        }
    }

    void DrawWalls()
    {
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin - 1; x <= bounds.xMax + 1; x++)
        {
            for (int y = bounds.yMin - 1; y <= bounds.yMax + 1; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (tilemap.GetTile(pos) != null) continue;

                bool nearFloor = false;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        if (tilemap.GetTile(pos + new Vector3Int(dx, dy, 0)) == floorTile)
                        {
                            nearFloor = true;
                            break;
                        }
                    }
                    if (nearFloor) break;
                }

                if (nearFloor)
                    wallTilemap.SetTile(pos, wallTile);
            }
        }
    }

    void SetCharacter()
    {
        player.transform.position = finalRooms[0].center;
        // 카메라 세팅
        TestCam testCam = FindObjectOfType<TestCam>();
        testCam.SetMap(finalRooms[0]);
        testCam.CamState = TestCam.ECameraState.InRoom;

        // BFS
        Queue<int> queue = new Queue<int>();
        bool[] visited = new bool[finalRooms.Count]; // 방문 처리
        int[] moveCnt = new int[finalRooms.Count]; // 몇 번 이동 해야 하는가

        queue.Enqueue(0);
        visited[0] = true;
        moveCnt[0] = 0;

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();
            foreach (var next in connectionMap[current]) // current와 연결된 맵 모두 체크
            {
                if (visited[next]) continue; // 다음 방문해야 할 맵에 이미 방문 했다면 continue;

                visited[next] = true;
                moveCnt[next] = moveCnt[current] + 1; // 이동 횟수 + 1
                queue.Enqueue(next);
            }
        }

        // 최다 이동 횟수 중 가장 거리가 긴 곳을 보스방으로
        int maxMoveCnt = -1;
        int bossRoomIdx = -1;
        float maxDist = -1;
        for (int i = 0; i < finalRooms.Count; i++)
        {
            float tmpDist = Vector2.Distance(finalRooms[0].center, finalRooms[i].center);
            if (moveCnt[i] > maxMoveCnt || (moveCnt[i] == maxMoveCnt && tmpDist > maxDist))
            {
                maxMoveCnt = moveCnt[i];
                bossRoomIdx = i;
                maxDist = tmpDist;
            }
        }
        boss.transform.position = finalRooms[bossRoomIdx].center;
        boss.GetComponent<Enemy>().MyRoom.Room = finalRooms[bossRoomIdx];
        boss.GetComponent<Enemy>().MyRoom.RoomIdx = bossRoomIdx;
        // 보스 방 입구 벽으로 막기
        SetBossRoom(finalRooms[bossRoomIdx], bossWallTile);

        // 테스트로 캐릭터 옆으로 이동
        //boss.transform.position = player.transform.position + (Vector3)(Vector2.right * 3);
    }
    void SetObstacles()
    {
        foreach(var room in finalRooms)
        {
            // 몇개 설치할 것인가 -> 대충 최대 20개
            int obsCnt = Random.Range(0, spawnObstacleCount);
            for (int i = 0; i < obsCnt; i++)
            {
                // 소환할 위치
                Vector3 pos = new Vector3();
                pos.x = Random.Range(room.min.x + 1, room.max.x - 1) + 0.5f;
                pos.y = Random.Range(room.min.y + 1, room.max.y - 1) + 0.5f;
                // 장애물 인덱스 선택
                int obsIdx = Random.Range(0, obstacles.Count);
                // 지정 위치에 소환
                spawnObstacles.Add(Instantiate(obstacles[obsIdx], pos, Quaternion.identity));
            }
        }
    }
    public RectInt GetRoomByPos(Vector3 pos)
    {
        Vector2Int tmpPos = new Vector2Int();
        tmpPos.x = (int)pos.x;
        tmpPos.y = (int)pos.y;
        
        //Debug.Log(tmpPos);
        foreach(var room in finalRooms)
        {
            // 판정을 위해 상 하 좌우 n 칸씩 확대
            int roomPadding = 2;
            RectInt expanded = new RectInt(room.xMin - roomPadding, room.yMin - roomPadding, room.width + roomPadding * 2, room.height + roomPadding * 2);
            if (expanded.Contains(tmpPos))
            {
                Enemy testBoss = boss.GetComponent<Enemy>();
                if (testBoss?.MyRoom.Room.center == room.center) // 보스방이라면 보스 활성화
                {
                    testBoss.BossState = Enemy.EBossState.Active;
                    CloseBossRoom();
                }
                return room;
            }
        }

        // 그럴리는 없겠지만 방을 못찾았다면 멀리 있는 값 리턴
        return new RectInt(666, 666, 0, 0);
    }
    void SetBossRoom(RectInt bossRoom, TileBase wallTile)
    {
        if (bossWallMap == null) return;

        // 버전 1: 가장 쉬운 버전, 벽으로 둘러싸기
        for (int i = bossRoom.min.x - 1; i < bossRoom.max.x + 1; i++)
        {
            bossWallMap.SetTile(new Vector3Int(i, bossRoom.min.y - 1, 0), wallTile);
            bossWallMap.SetTile(new Vector3Int(i, bossRoom.max.y, 0), wallTile);
        }
        for (int j = bossRoom.min.y - 1; j < bossRoom.max.y + 1; j++)
        {
            bossWallMap.SetTile(new Vector3Int(bossRoom.min.x - 1, j, 0), wallTile);
            bossWallMap.SetTile(new Vector3Int(bossRoom.max.x, j, 0), wallTile);

        }
    }

    // 통합해서 SetBossRoomDoor(bool active) 로??
    void OpenBossRoom()
    {
        bossWallMap.gameObject.SetActive(false);
    }
    void CloseBossRoom()
    {
        bossWallMap.gameObject.SetActive(true);
    }
    public void SetBossDeActive()
    {
        // 모든 보스 비활성화 하기.
        Enemy testBoss = boss.GetComponent<Enemy>();
        if (testBoss == null) return;
        testBoss.BossState = Enemy.EBossState.Deactivate;

    }
}

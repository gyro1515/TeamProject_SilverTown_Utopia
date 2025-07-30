using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerater : MonoBehaviour
{
    [Header("Map Settings")]
    public int totalCandidates = 50;
    public int desiredRoomCount = 20;
    public Vector2Int roomMinSize = new Vector2Int(6, 6);
    public Vector2Int roomMaxSize = new Vector2Int(12, 12);
    public int roomSpacing = 3;

    [Header("Tilemap Settings")]
    public Tilemap tilemap;
    public Tilemap wallTilemap;
    public TileBase floorTile;
    public TileBase wallTile;

    [Header("Character Setting")]
    public GameObject player;
    public GameObject boss;

    [Header("DrawLine Setting")]
    public float time = 4.0f;

    private List<RectInt> candidateRooms = new List<RectInt>();
    private List<RectInt> finalRooms = new List<RectInt>();
    private List<Vector2Int> roomCenters = new List<Vector2Int>();
    private HashSet<(int, int)> connections = new HashSet<(int, int)>();
    private Dictionary<int, List<int>> connectionMap = new Dictionary<int, List<int>>();

    void Start() => GenerateMap();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GenerateMap();
        }
    }

    void GenerateMap()
    {
        tilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        candidateRooms.Clear();
        finalRooms.Clear();
        roomCenters.Clear();
        connections.Clear();
        connectionMap.Clear();

        GenerateInitialRooms();
        ResolveRoomCollisions();
        FilterAndSelectRooms();
        ConnectRooms();
        DrawRooms();

        DrawCorridors();
        DrawWalls();

        SetCharacter();
    }

    void GenerateInitialRooms()
    {
        for (int i = 0; i < totalCandidates; i++)
        {
            int w = Random.Range(roomMinSize.x, roomMaxSize.x);
            int h = Random.Range(roomMinSize.y, roomMaxSize.y);
            int x = Random.Range(-5 - w / 2, 5 - w / 2); // 중심 맞추기
            int y = Random.Range(-5 - h / 2, 5 - h / 2); // 중심 맞추기
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
        for (int i = 0; i < finalRooms.Count; i++)
        {
            Vector2Int center = new Vector2Int((int)finalRooms[i].center.x, (int)finalRooms[i].center.y);
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
                    if (dist < minDist) // 최단거리이며, 연결 안된 룸인지 체크
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
            tilemap.SetTile(new Vector3Int(tilePos.x, tilePos.y, 0), floorTile);
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

    bool IsInAnyRoom(Vector2Int pos)
    {
        foreach (var room in finalRooms)
            if (room.Contains(pos)) return true;
        return false;
    }



    void SetCharacter()
    {
        player.transform.position = finalRooms[0].center;
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
            foreach (var neighbor in connectionMap[current]) // current와 연결된 맵 모두 체크
            {
                if (visited[neighbor]) continue; // 다음 방문해야 할 맵에 이미 방문 했다면 continue;

                visited[neighbor] = true;
                moveCnt[neighbor] = moveCnt[current] + 1; // 이동 횟수 + 1
                queue.Enqueue(neighbor);
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
        Debug.Log(maxMoveCnt);

    }
}

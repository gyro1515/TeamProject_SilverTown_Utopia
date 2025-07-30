using System.Collections;
using System.Collections.Generic;
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

    private List<RectInt> candidateRooms = new List<RectInt>();
    private List<RectInt> finalRooms = new List<RectInt>();
    private List<Vector2Int> roomCenters = new List<Vector2Int>();
    private HashSet<(int, int)> connections = new HashSet<(int, int)>();
    private Dictionary<int, List<int>> connectionMap = new Dictionary<int, List<int>>();

    void Start()
    {
        GenerateMap();
    }

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
        EnsureMinTwoConnections();
        DrawRooms();
        DrawCorridors();
        DrawWalls();
    }

    void GenerateInitialRooms()
    {
        for (int i = 0; i < totalCandidates; i++)
        {
            int w = Random.Range(roomMinSize.x, roomMaxSize.x);
            int h = Random.Range(roomMinSize.y, roomMaxSize.y);
            int x = Random.Range(-10 - w / 2, 10 - w / 2);
            int y = Random.Range(-10 - h / 2, 10 - h / 2);
            candidateRooms.Add(new RectInt(x, y, w, h));
        }
    }

    void ResolveRoomCollisions()
    {
        bool moved;
        do
        {
            moved = false;
            for (int i = 0; i < candidateRooms.Count; i++)
            {
                RectInt a = candidateRooms[i];
                Vector2 centerA = (Vector2)a.center;
                for (int j = 0; j < candidateRooms.Count; j++)
                {
                    if (i == j) continue;
                    RectInt b = candidateRooms[j];
                    RectInt expandedB = new RectInt(b.xMin - roomSpacing, b.yMin - roomSpacing, b.width + roomSpacing * 2, b.height + roomSpacing * 2);
                    if (a.Overlaps(expandedB))
                    {
                        Vector2 centerB = (Vector2)b.center;
                        Vector2 dir = (centerA - centerB).normalized;
                        if (dir == Vector2.zero)
                            dir = new Vector2(Random.value - 0.5f, Random.value - 0.5f).normalized;
                        a.position += Vector2Int.RoundToInt(dir);
                        candidateRooms[i] = a;
                        moved = true;
                    }
                }
            }
        } while (moved);
    }

    void FilterAndSelectRooms()
    {
        List<RectInt> largeEnough = new List<RectInt>();
        foreach (var room in candidateRooms)
        {
            if (room.width >= roomMinSize.x && room.height >= roomMinSize.y)
                largeEnough.Add(room);
        }

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
                    if (dist < minDist)
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
            }
        }
    }

    void EnsureMinTwoConnections()
    {
        int roomCount = roomCenters.Count;

        for (int i = 0; i < roomCount; i++)
        {
            HashSet<int> alreadyConnected = new HashSet<int>(connectionMap[i]);

            while (connectionMap[i].Count < 2)
            {
                int nearest = -1;
                float minDist = float.MaxValue;

                for (int j = 0; j < roomCount; j++)
                {
                    if (i == j) continue;
                    if (alreadyConnected.Contains(j)) continue;

                    float dist = Vector2Int.Distance(roomCenters[i], roomCenters[j]);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearest = j;
                    }
                }

                if (nearest != -1)
                {
                    AddConnection(i, nearest);
                    alreadyConnected.Add(nearest);
                }
                else
                {
                    break;
                }
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
            Vector2Int fromCenter = roomCenters[i];
            Vector2Int toCenter = roomCenters[j];

            Vector2Int from = GetCorridorStartPoint(fromCenter, toCenter);
            Vector2Int to = GetCorridorStartPoint(toCenter, fromCenter);

            Vector2Int pivot = Random.value < 0.5f ? new Vector2Int(to.x, from.y) : new Vector2Int(from.x, to.y);
            DrawWideCorridor(from, pivot);
            DrawWideCorridor(pivot, to);
        }
    }

    Vector2Int GetCorridorStartPoint(Vector2Int roomCenter, Vector2Int toward)
    {
        Vector2 dir = (toward - roomCenter);
        dir = dir.normalized * 2;
        Vector2Int dirInt = new Vector2Int();
        dirInt.x = (int)dir.x;
        dirInt.y = (int)dir.y;

        Vector2Int offset = dirInt;
        return roomCenter + offset;
    }

    void DrawWideCorridor(Vector2Int from, Vector2Int to)
    {
        Vector2Int current = from;
        Vector2Int dir = new Vector2Int(
            Mathf.Clamp(to.x - from.x, -1, 1),
            Mathf.Clamp(to.y - from.y, -1, 1)
        );

        while (current != to)
        {
            PlaceWideCorridorTiles(current, dir);
            current += dir;
        }
        PlaceWideCorridorTiles(to, dir);
    }

    void PlaceWideCorridorTiles(Vector2Int pos, Vector2Int dir)
    {
        tilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), floorTile);
        tilemap.SetTile(new Vector3Int(pos.x + dir.x, pos.y + dir.y, 0), floorTile);

        Vector2Int perp = new Vector2Int(-dir.y, dir.x);
        wallTilemap.SetTile(new Vector3Int(pos.x + perp.x, pos.y + perp.y, 0), wallTile);
        wallTilemap.SetTile(new Vector3Int(pos.x - perp.x, pos.y - perp.y, 0), wallTile);
    }

    void DrawWalls()
    {
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.xMin - 1; x <= bounds.xMax + 1; x++)
        {
            for (int y = bounds.yMin - 1; y <= bounds.yMax + 1; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (tilemap.GetTile(pos) == null)
                {
                    bool adjacentToFloor = false;
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;
                            if (tilemap.GetTile(pos + new Vector3Int(dx, dy, 0)) == floorTile)
                            {
                                adjacentToFloor = true;
                                break;
                            }
                        }
                        if (adjacentToFloor) break;
                    }

                    if (adjacentToFloor)
                    {
                        wallTilemap.SetTile(pos, wallTile);
                    }
                }
            }
        }
    }
}

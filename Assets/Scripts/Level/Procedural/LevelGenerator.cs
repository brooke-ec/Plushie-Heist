using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private LevelTile entranceTile;
    [SerializeField] private LevelTile exitTile;
    [SerializeField] private int minRoom = 4;
    [SerializeField] private int maxRoom = 9;
    [SerializeField] private int growAmount = 4;
    [SerializeField] private float tileSize = 5;
    [SerializeField] private float seedDistance = 2;
    [SerializeField] private Vector2Int size = new(24, 24);
    [SerializeField] private float sideRoomChance = 0.5f;
    [SerializeField] private int corridorWidth = 1;

    private Vector3 physicalSize => new Vector3(size.x - 1, 0, size.y - 1) * tileSize;

    private List<LevelRoom> rooms = new();
    private List<RoomEdge> edges = new();
    private Dictionary<Vector2Int, TilePlacement> grid = new();
    private Queue<Vector2Int> toPlace = new();

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, physicalSize);
    }
#endif

    private void Start()
    {
        rooms.Add(new LevelRoom().FromSize(0, 0, minRoom, minRoom));
        rooms.Add(new LevelRoom().FromSize(size.x - minRoom, size.y - minRoom, minRoom, minRoom));

        Seed();
        
        rooms.Reverse();

        foreach (var box in rooms)
        {
            box.Grow(rooms, growAmount);
            box.Clamp(size);
        }

        int length = rooms.Count;
        for (int i = 0; i < length; i++) Split(rooms[i]);

        rooms = rooms.Where((b) => b.size.x >= minRoom && b.size.y >= minRoom).ToList();
        rooms.Reverse();

        foreach (var box in rooms)
        {
            box.Grow(rooms, growAmount);
            box.Clamp(size);
        }

        length = rooms.Count;
        for (int i = 0; i < length; i++) Split(rooms[i]);

        FindEdges();
        //Display(edges, 1);

        CalculateDistances(rooms.First((r) => r.bottom == 0 && r.left == 0));
        ConnectPath(rooms.First((r) => r.top == size.y && r.right == size.x));


        foreach (var box in rooms) box.Shrink();
        foreach (var edge in edges) edge.Collapse(corridorWidth);
        //Display(edges.Where((e) => e.connected).ToList(), 0, startTile);

        //Display(rooms, 0, startTile2);
        //Display(rooms.Where((e) => e.pathDistance != -1).ToList(), 0, startTile2);


        rooms.Where((e) => e.pathDistance != -1).ForEach(r => r.ForEach(v => grid[v] = null));
        edges.Where((e) => e.connected).ForEach(r => r.ForEach(v => grid[v] = null));
        PlaceTiles();
    }

    private void PlaceTiles()
    {
        PlaceTile(entranceTile, Vector2Int.one);
        PlaceTile(entranceTile, size - Vector2Int.one);

        while (toPlace.Count > 0)
        {
            Vector2Int position = toPlace.Dequeue();
            if (grid[position] != null) continue;

            Vector2Int v;
            TileNeighbors neighbors = new TileNeighbors(
                position,
                new TileNeighbour(grid.GetValueOrDefault(v = position + new Vector2Int(+1, 0)), !grid.ContainsKey(v)),
                new TileNeighbour(grid.GetValueOrDefault(v = position + new Vector2Int(0, +1)), !grid.ContainsKey(v)),
                new TileNeighbour(grid.GetValueOrDefault(v = position + new Vector2Int(-1, 0)), !grid.ContainsKey(v)),
                new TileNeighbour(grid.GetValueOrDefault(v = position + new Vector2Int(0, -1)), !grid.ContainsKey(v))
            );

            TilePlacement[] possible = neighbors.Possible();
            if (possible.Length > 0) PlaceTile(possible[Random.Range(0, possible.Length)]);
            else throw new System.IndexOutOfRangeException($"Found no options for placing tile");
        }
    }

    private void PlaceTile(LevelTile tile, Vector2Int position)
    {
        PlaceTile(new TilePlacement(tile, position, 0));
    }

    private void PlaceTile(TilePlacement placement)
    {
        grid[placement.position] = placement;
        Instantiate(placement.tile, new Vector3(placement.position.x - .5f, 0, placement.position.y - .5f) * tileSize - physicalSize / 2, placement.quaternion, transform);

        Vector2Int v;
        if (grid.ContainsKey(v = placement.position + new Vector2Int(+1, 0)) && !toPlace.Contains(v)) toPlace.Enqueue(v);
        if (grid.ContainsKey(v = placement.position + new Vector2Int(0, +1)) && !toPlace.Contains(v)) toPlace.Enqueue(v);
        if (grid.ContainsKey(v = placement.position + new Vector2Int(-1, 0)) && !toPlace.Contains(v)) toPlace.Enqueue(v);
        if (grid.ContainsKey(v = placement.position + new Vector2Int(0, -1)) && !toPlace.Contains(v)) toPlace.Enqueue(v);
    }

    private void ConnectPath(LevelRoom destination)
    {
        LevelRoom current = destination;
        if (current.distance == -1) throw new System.Exception("No path from start to finish");
        current.pathDistance = 0;

        RoomEdge previous = null;
        while (current.distance > 0)
        {
            current.backtrack.connected = true;
            current = current.backtrack.GetOther(current);
            
            current.pathDistance = 0;
            foreach (RoomEdge edge in current.edges)
            {
                if (edge == current.backtrack || edge == previous) continue;
                if (Random.Range(0f, 1f) > sideRoomChance) continue;
                edge.connected = true;
                LevelRoom room = edge.GetOther(current);
                room.pathDistance = 1;

                foreach (RoomEdge e in room.edges)
                {
                    LevelRoom other = e.GetOther(room);
                    if (other.pathDistance == 1 || other.pathDistance == 0) e.connected = true;
                }
            }

            previous = current.backtrack;
        }
    }

    private void CalculateDistances(LevelRoom start)
    {
        Queue<FindPathParameters> queue = new();
        queue.Enqueue(new FindPathParameters(start, 0, null));

        while (queue.Count > 0)
        {
            FindPathParameters p = queue.Dequeue();
            if (p.room.distance != -1) continue;
            
            p.room.distance = p.distance;
            p.room.backtrack = p.backtrack;

            foreach (RoomEdge r in p.room.edges) queue.Enqueue(new FindPathParameters(r.GetOther(p.room), p.distance + 1, r));
        }
    }

    private void FindEdges()
    {
        for (int i = 0; i < rooms.Count; i++)
            for (int j = i + 1;  j < rooms.Count; j++)
            {
                IntersectResult hit = rooms[i].Intersect(rooms[j]);
                if (hit.separation != 0) continue;

                RoomEdge edge = new RoomEdge();
                edge.roomA = rooms[i];
                edge.roomB = rooms[j];

                if (hit.dirX == 1)
                {
                    edge.left = rooms[i].right;
                    edge.right = rooms[j].left + 1;
                    edge.bottom = Mathf.Max(rooms[i].bottom, rooms[j].bottom) + 1;
                    edge.top = Mathf.Min(rooms[i].top, rooms[j].top);
                } else if (hit.dirX == -1)
                {
                    edge.left = rooms[j].right;
                    edge.right = rooms[i].left + 1;
                    edge.bottom = Mathf.Max(rooms[i].bottom, rooms[j].bottom) + 1;
                    edge.top = Mathf.Min(rooms[i].top, rooms[j].top);
                } else if (hit.dirY == 1)
                {
                    edge.bottom = rooms[i].top;
                    edge.top = rooms[j].bottom + 1;
                    edge.left = Mathf.Max(rooms[i].left, rooms[j].left) + 1;
                    edge.right = Mathf.Min(rooms[i].right, rooms[j].right);
                } else if (hit.dirY == -1)
                {
                    edge.bottom = rooms[j].top;
                    edge.top = rooms[i].bottom + 1;
                    edge.left = Mathf.Max(rooms[i].left, rooms[j].left) + 1;
                    edge.right = Mathf.Min(rooms[i].right, rooms[j].right);
                }

                if (edge.left >= edge.right || edge.bottom >= edge.top) continue;

                edge.connected = false;
                rooms[i].edges.Add(edge);
                rooms[j].edges.Add(edge);
                edges.Add(edge);
            }
    }

    private void Split(LevelRoom box)
    {
        if (box.size.x > maxRoom)
        {
            int cut = Random.Range(box.left + minRoom, box.right + 1 - minRoom);
            LevelRoom newbox = new LevelRoom().FromBounds(cut, box.top, box.right, box.bottom);
            rooms.Add(newbox);
            Split(newbox);

            box.right = cut;
            Split(box);
        }
        else if (box.size.y > maxRoom)
        {
            int cut = Random.Range(box.bottom + minRoom, box.top + 1 - minRoom);
            LevelRoom newbox = new LevelRoom().FromBounds(box.left, box.top, box.right, cut);
            rooms.Add(newbox);
            Split(newbox);

            box.top = cut;
            Split(box);
        }
    }

    private void Seed()
    {
        for (int i = 0; i < size.x * size.y; i++)
        {
            LevelRoom box = new LevelRoom().FromPoint(Random.Range(0, size.x), Random.Range(0, size.y));

            float separation = rooms.Select((b) => box.Intersect(b).separation).Min();
            if (separation < seedDistance) continue;

            rooms.Add(box);
        }
    }

    private record FindPathParameters(LevelRoom room, int distance, RoomEdge backtrack) { }
}

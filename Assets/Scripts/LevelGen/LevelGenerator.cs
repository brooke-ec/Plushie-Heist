using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

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

    private List<Room> rooms = new();
    private List<Edge> edges = new();
    private Dictionary<Vector2Int, LevelTile> grid = new();
    private Queue<Vector2Int> toPlace = new();

    private void Start()
    {
        rooms.Add(Room.FromSize(0, 0, minRoom, minRoom));
        rooms.Add(Room.FromSize(size.x - minRoom, size.y - minRoom, minRoom, minRoom));

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
        PlaceTile(exitTile, new Vector2Int(size.x - 1, size.y - 1));

        while (toPlace.Count > 0)
        {
            Vector2Int position = toPlace.Dequeue();
            if (grid[position] != null) continue;

            LevelTile positiveX = grid.GetValueOrDefault(position + new Vector2Int(+1, 0));
            LevelTile negativeX = grid.GetValueOrDefault(position + new Vector2Int(-1, 0));
            LevelTile positiveZ = grid.GetValueOrDefault(position + new Vector2Int(0, +1));
            LevelTile negativeZ = grid.GetValueOrDefault(position + new Vector2Int(0, -1));

            List<LevelTile> possible = new();
            if (positiveX != null) possible.AddRange(positiveX.negativeX);
            if (negativeX != null) possible.AddRange(negativeX.positiveX);
            if (positiveZ != null) possible.AddRange(positiveZ.negativeZ);
            if (negativeZ != null) possible.AddRange(negativeZ.positiveZ);

            possible = possible.Where(t => {
                return t != null
                && t.positiveX.Contains(positiveX)
                && t.negativeX.Contains(negativeX)
                && t.positiveZ.Contains(positiveZ)
                && t.negativeZ.Contains(negativeZ);
            }).ToList();

            PlaceTile(possible[Random.Range(0, possible.Count)], position);
        }
    }

    private void PlaceTile(LevelTile tile, Vector2Int position)
    {
        grid[position] = tile;
        Instantiate(tile, new Vector3(position.x, 0, position.y) * tileSize, Quaternion.identity, transform);

        Vector2Int v;
        if (grid.ContainsKey(v = position + new Vector2Int(-1, 0)) && !toPlace.Contains(v)) toPlace.Enqueue(v);
        if (grid.ContainsKey(v = position + new Vector2Int(+1, 0)) && !toPlace.Contains(v)) toPlace.Enqueue(v);
        if (grid.ContainsKey(v = position + new Vector2Int(0, -1)) && !toPlace.Contains(v)) toPlace.Enqueue(v);
        if (grid.ContainsKey(v = position + new Vector2Int(0, +1)) && !toPlace.Contains(v)) toPlace.Enqueue(v);
    }

    private void ConnectPath(Room destination)
    {
        Room current = destination;
        if (current.distance == -1) throw new System.Exception("No path from start to finish");
        current.pathDistance = 0;

        Edge previous = null;
        while (current.distance > 0)
        {
            current.backtrack.connected = true;
            current = current.backtrack.GetOther(current);
            
            current.pathDistance = 0;
            foreach (Edge edge in current.edges)
            {
                if (edge == current.backtrack || edge == previous) continue;
                if (Random.Range(0f, 1f) > sideRoomChance) continue;
                edge.connected = true;
                Room room = edge.GetOther(current);
                room.pathDistance = 1;

                foreach (Edge e in room.edges)
                {
                    Room other = e.GetOther(room);
                    if (other.pathDistance == 1 || other.pathDistance == 0) e.connected = true;
                }
            }

            previous = current.backtrack;
        }
    }

    private void CalculateDistances(Room start)
    {
        Queue<FindPathParameters> queue = new();
        queue.Enqueue(new FindPathParameters(start, 0, null));

        while (queue.Count > 0)
        {
            FindPathParameters p = queue.Dequeue();
            if (p.room.distance != -1) continue;
            
            p.room.distance = p.distance;
            p.room.backtrack = p.backtrack;

            foreach (Edge r in p.room.edges) 
                queue.Enqueue(new FindPathParameters(r.GetOther(p.room), p.distance + 1, r));
        }
    }

    private void FindEdges()
    {
        for (int i = 0; i < rooms.Count; i++)
            for (int j = i + 1;  j < rooms.Count; j++)
            {
                IntersectHit hit = rooms[i].Intersects(rooms[j]);
                if (hit.separation != 0) continue;

                Edge edge = new Edge();
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

    private void Split(Room box)
    {
        if (box.size.x > maxRoom)
        {
            int cut = Random.Range(box.left + minRoom, box.right + 1 - minRoom);
            Room newbox = Room.FromBounds(cut, box.top, box.right, box.bottom);
            rooms.Add(newbox);
            Split(newbox);

            box.right = cut;
            Split(box);
        }
        else if (box.size.y > maxRoom)
        {
            int cut = Random.Range(box.bottom + minRoom, box.top + 1 - minRoom);
            Room newbox = Room.FromBounds(box.left, box.top, box.right, cut);
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
            Room box = Room.FromPoint(Random.Range(0, size.x), Random.Range(0, size.y));

            float separation = rooms.Select((b) => box.Intersects(b).separation).Min();
            if (separation < seedDistance) continue;

            rooms.Add(box);
        }
    }

    private record FindPathParameters
    {
        public Room room;
        public int distance;
        public Edge backtrack;

        public FindPathParameters(Room room, int distance, Edge backtrack)
        {
            this.room = room;
            this.distance = distance;
            this.backtrack = backtrack;
        }
    }

    private abstract class Region : IEnumerable<Vector2Int>
    {
        public int top, right, bottom, left;

        public IEnumerator<Vector2Int> GetEnumerator()
        {
            for (int x = left; x < right; x++)
                for (int y = bottom; y < top; y++)
                    yield return new Vector2Int(x, y);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    private class Edge : Region
    {
        public bool connected;
        public Room roomA;
        public Room roomB;

        public Room GetOther(Room self)
        {
            if (self == roomA) return roomB;
            if (self == roomB) return roomA;
            else throw new System.Exception("Room not in Edge");
        }

        public void Collapse(int width)
        {
            if (right - left >= width)
            {
                left = Random.Range(left, right - width + 1);
                right = left + width;
            }

            if (top - bottom >= width)
            {
                bottom = Random.Range(bottom, top - width + 1);
                top = bottom + width;
            }
        }
    }

    private class Room : Region
    {
        public List<Edge> edges = new List<Edge>();
        public int pathDistance = -1;
        public int distance = -1;
        public Edge backtrack;

        public Vector2 center => new Vector2(right + left, top + bottom) * 0.5f;
        public Vector2Int size => new Vector2Int(right - left, top - bottom);
        public Vector2 half => new Vector2(size.x, size.y) * 0.5f;

        private Room(int x1, int y1, int x2, int y2)
        {
            this.top = Mathf.Max(y1, y2);
            this.right = Mathf.Max(x1, x2);
            this.bottom = Mathf.Min(y1, y2);
            this.left = Mathf.Min(x1, x2);
        }

        public static Room FromBounds(int x1, int y1, int x2, int y2)
        {
            return new Room(x1, y1, x2, y2);
        }

        public static Room FromSize(int x, int y, int width, int height)
        {
            return new Room(x, y, x + width, y + height);
        }

        public static Room FromPoint(int x, int y)
        {
            return Room.FromSize(x, y, 1, 1);
        }

        public IntersectHit Intersects(Room other)
        {
            float dx = other.center.x - this.center.x;
            float px = (other.half.x + this.half.x) - Mathf.Abs(dx);

            float dy = other.center.y - this.center.y;
            float py = (other.half.y + this.half.y) - Mathf.Abs(dy);

            IntersectHit hit = new IntersectHit(px > 0 && py > 0);

            if (px < py)
            {
                hit.dirX = (int) Mathf.Sign(dx);
                hit.deltaX = (int) px * hit.dirX;
                hit.separation = (int) -px;
            } else
            {
                hit.dirY = (int) Mathf.Sign(dy);
                hit.deltaY = (int) py * hit.dirY;
                hit.separation = (int) -py;
            }

            return hit;
        }

        public void Grow(List<Room> boxes, int maxGrow)
        {
            int up = maxGrow;
            int right = maxGrow;
            int down = -maxGrow;
            int left = -maxGrow;

            foreach (Room box in boxes)
            {
                if (box == this) continue;

                IntersectHit hit = this.Intersects(box);

                if (hit.hit) Debug.LogError("Overlapping boxes! D:");

                if (hit.dirX > 0) right = Mathf.Min(right, -hit.deltaX);
                else if (hit.dirX < 0) left = Mathf.Max(left, -hit.deltaX);
                
                if (hit.dirY > 0) up = Mathf.Min(up, -hit.deltaY);
                else if (hit.dirY < 0) down = Mathf.Max(down, -hit.deltaY);
            }

            this.top += up;
            this.right += right;
            this.bottom += down;
            this.left += left;
        }

        public void Clamp(Vector2Int size)
        {
            top = Mathf.Min(top, size.y);
            right = Mathf.Min(right, size.x);
            bottom = Mathf.Max(bottom, 0);
            left = Mathf.Max(left, 0);
        }

        public void Shrink()
        {
            left += 1;
            bottom += 1;
        }
    }

    private struct IntersectHit
    {
        public int deltaX, deltaY;
        public int dirX, dirY;
        public int separation;
        public bool hit;

        public readonly Vector2Int delta => new Vector2Int(deltaX, deltaY);

        public IntersectHit(bool hit)
        {
            this.separation = 0;
            this.hit = hit;
            deltaX = 0;
            deltaY = 0;
            dirX = 0;
            dirY = 0;
        }
    }
}

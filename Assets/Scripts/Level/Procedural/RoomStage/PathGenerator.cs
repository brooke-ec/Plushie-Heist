using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PathGenerator
{
    [SerializeField] private float sideRoomChance = 0.5f;
    [SerializeField] private float collapseChance = 0.5f;
    [SerializeField] private int corridorWidth = 1;

    private LevelRoom start => rooms.First((r) => r.bottom == 0 && r.left == 0);
    private LevelRoom end => rooms.OrderByDescending(r => r.top + r.right).First();

    private List<RoomEdge> edges;
    private LevelRoom[] rooms;

    public RoomEdge[] Generate(LevelRoom[] rooms)
    {
        edges = new List<RoomEdge>();
        this.rooms = rooms;

        FindEdges();
        CalculateDistances(start);
        ConnectPath(end);

        // Collapse edges based on chance
        foreach (RoomEdge edge in edges) if (Random.Range(0f, 1f) < collapseChance) edge.Collapse(corridorWidth);

        return edges.ToArray();
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

    private void FindEdges()
    {
        for (int i = 0; i < rooms.Length; i++)
            for (int j = i + 1; j < rooms.Length; j++)
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
                }
                else if (hit.dirX == -1)
                {
                    edge.left = rooms[j].right;
                    edge.right = rooms[i].left + 1;
                    edge.bottom = Mathf.Max(rooms[i].bottom, rooms[j].bottom) + 1;
                    edge.top = Mathf.Min(rooms[i].top, rooms[j].top);
                }
                else if (hit.dirY == 1)
                {
                    edge.bottom = rooms[i].top;
                    edge.top = rooms[j].bottom + 1;
                    edge.left = Mathf.Max(rooms[i].left, rooms[j].left) + 1;
                    edge.right = Mathf.Min(rooms[i].right, rooms[j].right);
                }
                else if (hit.dirY == -1)
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

    private record FindPathParameters(LevelRoom room, int distance, RoomEdge backtrack) { }
}
